
using System.Security.Cryptography;
using System.Text;

namespace Tozawa.Auth.Svc.Helper;

public class ScramshaProtocol
{
    private USER_MODE _user_mode = USER_MODE.INVALID;

    private string _user_name = string.Empty;
    private string _password = string.Empty;

    private string _client_first_message = string.Empty;
    private string _client_first_message_bare = string.Empty;
    private string _client_final_message = string.Empty;
    private string _client_final_message_without_proof = string.Empty;
    private string _server_first_message = string.Empty;
    private string _server_final_message = string.Empty;

    private string _authentication_message = string.Empty;
    private byte[] _client_key = null;
    private byte[] _client_signature = null;
    private byte[] _client_proof = null;
    private byte[] _salted_password = null;
    private byte[] _stored_key = null;
    private byte[] _server_key = null;
    private byte[] _server_signature = null;


    private string _client_nonce = string.Empty;
    private string _server_nonce = string.Empty;
    private string _client_server_nonce = string.Empty;
    private string _server_salt = string.Empty;
    private string _server_iteration = string.Empty;
    private string _client_proof_64 = string.Empty;
    private string _computed_server_signature_64 = string.Empty;
    private string _received_server_signature_64 = string.Empty;

    private byte[] _server_salt_byte = null;
    private int _salt_byte_length = 320;
    private int _nonce_byte_length = 32;
    private int _server_iteration_i = 0;
    private int _min_iteration_count = 4000;
    private int _max_iteration_count = 5000;
    private int _max_allowable_iteration_count = 10000;
    private const int _sha_output_length = 20; // this is the length of the output of SHA-1 HMAC
    private byte[] _random_crypt_prov = null;
    private Random _random_gen;

    //see https://gist.github.com/markokr/4654875 for test vectors
    private static bool _is_test = false;

    public ScramshaProtocol(USER_MODE user_mode, string user_name, string password) :
        this(user_mode, user_name, password, 32)
    {

    }
    public ScramshaProtocol(USER_MODE user_mode, string user_name, string password, int nonce_byte_length)
    {
        if (user_mode != USER_MODE.CLIENT)
            throw new ArgumentException("ScramshaProtocol:The user mode must be client");

        if (string.IsNullOrEmpty(user_name))
            throw new ArgumentException("ScramshaProtocol: User name cannot be null/empty ");

        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("ScramshaProtocol: Password cannot be null/empty ");

        if (nonce_byte_length < 1)
            throw new ArgumentException("ScramshaProtocol: The nonce byte length cannot be less than 1 ");

        _user_mode = user_mode;
        _user_name = user_name;
        _password = password;
        _nonce_byte_length = nonce_byte_length;

        _client_nonce = GetNonce();

        /*
        Tester values source: https://gist.github.com/markokr/4654875            
       */
        if (_is_test == true && _user_name.Equals("user") && _password.Equals("pencil"))
            _client_nonce = "fyko+d2lbbFgONRv9qkxdawL";

        _client_first_message = "n,,n=" + _user_name + ",r=" + _client_nonce;
        _client_first_message_bare = "n=" + _user_name + ",r=" + _client_nonce;
    }

    public ScramshaProtocol(USER_MODE user_mode, string client_first_message) :
        this(user_mode, client_first_message, 32, 320)
    {
    }


    public ScramshaProtocol(USER_MODE user_mode, string client_first_message, int nonce_byte_length, int salt_byte_length)
    {
        if (user_mode != USER_MODE.SERVER)
            throw new ArgumentException("ScramshaProtocol:The user mode must be server");

        if (string.IsNullOrEmpty(client_first_message))
            throw new ArgumentException("ScramshaProtocol: Client first message string cannot be null/empty ");

        if (nonce_byte_length < 1)
            throw new ArgumentException("ScramshaProtocol: The nonce byte length cannot be less than 1 ");

        if (salt_byte_length < 1)
            throw new ArgumentException("ScramshaProtocol: The salt byte length cannot be less than 1 ");

        _user_mode = user_mode;
        _nonce_byte_length = nonce_byte_length;
        _salt_byte_length = salt_byte_length;

        _client_first_message = client_first_message;

        if (SplitClientFirstMessage(_client_first_message) == false)
            throw new ArgumentException("ScramshaProtocol: Client first message not properly formatted ");
    }

    public string GetClientFirstMessage()
    {
        return _client_first_message;
    }
    public string GetClientFinalMessage(string server_first_message)
    {
        if (string.IsNullOrEmpty(server_first_message))
            return string.Empty;

        try
        {
            _server_first_message = server_first_message;

            if (SplitServerFirstMessage(_server_first_message) == false)
                return string.Empty;

            int _iteration_count = Convert.ToInt32(_server_iteration);

            if (_is_test == false && _server_iteration_i > _max_allowable_iteration_count)
                return string.Empty;

            string _normalized_password = _password.Normalize(NormalizationForm.FormKC);

            _salted_password = Hi(_normalized_password, Convert.FromBase64String(_server_salt), Convert.ToInt32(_iteration_count));

            _client_key = ComputeHMACHash(_salted_password, "Client Key");

            SHA1 _sha_1 = SHA1.Create();
            _stored_key = _sha_1.ComputeHash(_client_key);

            _client_final_message_without_proof = "c=" + EncodeTo64("n,,") + "," + _client_server_nonce;

            _authentication_message = _client_first_message_bare + "," + _server_first_message + "," + _client_final_message_without_proof;


            _client_signature = ComputeHMACHash(_stored_key, _authentication_message);

            _client_proof = GetClientProof(_client_key, _client_signature);


            _client_final_message = _client_final_message_without_proof + ",p=" + Convert.ToBase64String(_client_proof);


            _server_key = ComputeHMACHash(_salted_password, "Server Key");
            _server_signature = ComputeHMACHash(_server_key, _authentication_message);

            _computed_server_signature_64 = Convert.ToBase64String(_server_signature);
        }
        catch
        {
            _client_final_message = string.Empty;
        }
        return _client_final_message;
    }
    public bool VerifyServerSignature(string server_final_message)
    {
        bool _is_verified = false;

        if (string.IsNullOrEmpty(server_final_message))
            return _is_verified;

        try
        {
            _server_final_message = server_final_message;

            if (SplitServerFinalMessage(_server_final_message) == true)
                _is_verified = _received_server_signature_64.Equals(_computed_server_signature_64);
        }
        catch
        {
            _is_verified = false;
        }
        return _is_verified;
    }

    public string GetServerFirstMessage(string password)
    {
        if (string.IsNullOrEmpty(_server_first_message) == false)
            return _server_first_message;

        if (_user_mode == USER_MODE.CLIENT)
            return string.Empty;

        if (string.IsNullOrEmpty(_client_nonce))
            throw new ArgumentException("GetServerFirstMessage: The received client nounce cannot be null/empty");

        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("GetServerFirstMessage: Password cannot be null/empty ");

        _password = password;

        try
        {
            _server_nonce = GetNonce();

            _client_server_nonce = "r=" + _client_nonce + _server_nonce;
            _server_salt_byte = GetRandomByteArray(_salt_byte_length);
            _server_iteration_i = GetRandomInteger();
            _server_iteration = _server_iteration_i.ToString();
            _server_salt = Convert.ToBase64String(_server_salt_byte);

            /*
             Tester values source: https://gist.github.com/markokr/4654875            
            */
            if (_is_test == true && _user_name.Equals("user") && _password.Equals("pencil"))
            {
                _server_salt = "QSXCR+Q6sek8bf92";
                _server_salt_byte = Convert.FromBase64String(_server_salt);
                _server_iteration_i = 4096;
                _server_iteration = _server_iteration_i.ToString();
                _server_nonce = "3rfcNHYJY1ZVvWVs7j";
                _client_server_nonce = "r=" + _client_nonce + _server_nonce;
            }
            string _normalized_password = _password.Normalize(NormalizationForm.FormKC);
            _salted_password = Hi(_normalized_password, _server_salt_byte, _server_iteration_i);

            _client_key = ComputeHMACHash(_salted_password, "Client Key");
            _server_key = ComputeHMACHash(_salted_password, "Server Key");

            SHA1 _sha_1 = SHA1.Create();
            _stored_key = _sha_1.ComputeHash(_client_key);

            // e.g., r=fyko+d2lbbFgONRv9qkxdawL3rfcNHYJY1ZVvWVs7j,s=QSXCR+Q6sek8bf92,i=4096
            _server_first_message = _client_server_nonce + ",s=" + _server_salt + ",i=" + _server_iteration;
        }
        catch
        {
            _server_first_message = string.Empty;
        }
        return _server_first_message;
    }
    public string GetServerFinalMessage(string client_final_message, ref bool is_client_authenticated)
    {
        is_client_authenticated = false;

        if (string.IsNullOrEmpty(client_final_message))
            return string.Empty;

        _client_final_message = client_final_message;

        if (SplitClientFinalMessage(_client_final_message) == false)
            return string.Empty;

        if (_user_mode == USER_MODE.CLIENT)
            return string.Empty;
        try
        {
            _client_first_message_bare = "n=" + _user_name + ",r=" + _client_nonce;
            _authentication_message = _client_first_message_bare + "," + _server_first_message + "," + _client_final_message_without_proof;

            _client_signature = ComputeHMACHash(_stored_key, _authentication_message);
            _client_proof = GetClientProof(_client_key, _client_signature);

            string _rxed_client_proof = Convert.ToBase64String(_client_proof);

            if (_client_proof_64.Equals(_rxed_client_proof) == false)
                return string.Empty;
            else
                is_client_authenticated = true;

            _server_signature = ComputeHMACHash(_server_key, _authentication_message);

            _server_final_message = "v=" + Convert.ToBase64String(_server_signature);
        }
        catch
        {
            _server_final_message = string.Empty;
        }
        return _server_final_message;
    }


    public string GetUserName()
    {
        return _user_name;
    }

    public static void UseTestVectors(bool is_test)
    {
        _is_test = is_test;
    }
    public void SetIterationCountLimits(int min_iteration_count, int max_iteration_count)
    {
        if (_user_mode == USER_MODE.CLIENT)
            throw new ArgumentException("SetIterationCountLimits: This operation is not valid the client user mode");

        if (min_iteration_count < 1)
            throw new ArgumentException("The value of the min iteration count cannot be less than 1");

        if (max_iteration_count < 1)
            throw new ArgumentException("The value of the max iteration count cannot be less than 1");

        if (min_iteration_count > max_iteration_count)
            throw new ArgumentException("The value of the min iteration count cannot be greater than that of the max iteration count");

        _min_iteration_count = min_iteration_count;
        _max_iteration_count = max_iteration_count;
    }
    public void SetMaximumIterationCount(int max_allowable_iteration_count)
    {
        if (_user_mode == USER_MODE.SERVER)
            throw new ArgumentException("SetIterationCountLimits: This operation is not valid the client user mode");

        _max_allowable_iteration_count = max_allowable_iteration_count;
    }

    private bool SplitClientFirstMessage(string client_first_message)
    {
        if (string.IsNullOrEmpty(client_first_message))
            return false;

        string[] _string_parts = client_first_message.Split(',');

        if (_string_parts.Length < 4)
            return false;

        //If client first message does not contain n,p or y in the begining then it must fail. See Section 5 of the RFC.

        if (_string_parts[0].Equals("n") == false && _string_parts[0].Equals("p") == false && _string_parts[0].Equals("y") == false)
            return false;

        if (string.IsNullOrEmpty(_string_parts[1]) == false)
            return false;

        string[] _string_parts_2 = _string_parts[2].Split(new char[] { '=' }, 2);

        if (_string_parts_2.Length < 2)
            return false;

        if (_string_parts_2[0].Equals("n") == false)
            return false;

        _user_name = _string_parts_2[1];

        _string_parts_2 = _string_parts[3].Split(new char[] { '=' }, 2);

        if (_string_parts_2.Length < 2)
            return false;

        if (_string_parts_2[0].Equals("r") == false)
            return false;

        _client_nonce = _string_parts_2[1];

        return true;
    }
    private bool SplitServerFirstMessage(string server_first_message)
    {
        if (string.IsNullOrEmpty(server_first_message))
            return false;

        string[] _string_parts = _server_first_message.Split(',');

        if (_string_parts.Length < 3)
            return false;

        _client_server_nonce = _string_parts[0];
        _server_salt = _string_parts[1].Substring(2);
        _server_iteration = _string_parts[2].Substring(2);

        if (string.IsNullOrEmpty(_client_server_nonce) || string.IsNullOrEmpty(_server_salt)
            || string.IsNullOrEmpty(_server_iteration))
            return false;

        return true;
    }
    private bool SplitClientFinalMessage(string client_final_message)
    {
        if (string.IsNullOrEmpty(client_final_message))
            return false;

        string[] _string_parts = _client_final_message.Split(',');

        if (_string_parts.Length < 3)
            return false;

        if (_string_parts[0].Equals("c=biws") == false)
            return false;

        string[] _string_parts_2 = _string_parts[2].Split(new char[] { '=' }, 2);

        if (_string_parts_2.Length < 2)
            return false;

        if (_string_parts_2[0].Equals("p") == false)
            return false;

        _client_proof_64 = _string_parts_2[1];
        _client_final_message_without_proof = _string_parts[0] + "," + _client_server_nonce;

        return true;
    }
    private bool SplitServerFinalMessage(string server_final_message)
    {
        if (string.IsNullOrEmpty(server_final_message))
            return false;

        string[] _string_parts = server_final_message.Split(new char[] { '=' }, 2);

        if (_string_parts.Length < 2)
            return false;

        if (_string_parts[0].Equals("v") == false)
            return false;

        _received_server_signature_64 = _string_parts[1];
        return true;
    }

    private byte[] ComputeHMACHash(byte[] data, string key)
    {
        using (var _hmac_sha_1 = new HMACSHA1(data, true))
        {
            byte[] _hash_bytes = _hmac_sha_1.ComputeHash(Encoding.UTF8.GetBytes(key));
            return _hash_bytes;
        }
    }
    private byte[] Hi(string password, byte[] salt, int iteration_count)
    {
        Rfc2898DeriveBytes _pdb = new Rfc2898DeriveBytes(password, salt, iteration_count);

        return _pdb.GetBytes(_sha_output_length);
    }
    private string GetNonce()
    {
        return Convert.ToBase64String(GetRandomByteArray(_nonce_byte_length));
    }

    private byte[] GetRandomByteArray(int byte_array_length)
    {
        byte[] _random_byte_array = new byte[byte_array_length];

        if (_random_crypt_prov == null)
            _random_crypt_prov = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(_random_crypt_prov);
        }

        return _random_byte_array;
    }
    private int GetRandomInteger()
    {
        if (_random_gen == null)
            _random_gen = new Random();

        int _random_int = _random_gen.Next(_min_iteration_count, _max_iteration_count);

        if (_random_int < 0)
            _random_int *= -1;

        return _random_int;
    }
    private byte[] GetClientProof(byte[] buffer_a, byte[] buffer_b)
    {
        if (buffer_a == null || buffer_b == null || buffer_a.Length < 1 || buffer_b.Length < 1)
            return null;

        byte[] _buffer = new byte[buffer_a.Length];

        for (int i = 0; i < buffer_a.Length; ++i)
            _buffer[i] = (byte)(buffer_a[i] ^ buffer_b[i]);

        return _buffer;
    }
    private string DecodeFrom64(string string_to_decode)
    {

        if (string.IsNullOrEmpty(string_to_decode))
            return string.Empty;

        byte[] _bytes_decoded = Convert.FromBase64String(string_to_decode);

        return UTF8Encoding.UTF8.GetString(_bytes_decoded);
    }
    private string EncodeTo64(string string_to_encode)
    {

        if (string.IsNullOrEmpty(string_to_encode))
            return string.Empty;


        byte[] _byte_encoded = UTF8Encoding.UTF8.GetBytes(string_to_encode);

        return Convert.ToBase64String(_byte_encoded);

    }
}

public enum USER_MODE
{
    SERVER,
    CLIENT,
    INVALID,
    Default = INVALID
}