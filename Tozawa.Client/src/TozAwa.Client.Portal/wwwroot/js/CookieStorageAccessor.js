export function get() {
    return document.cookie;
}

export function getDecrypted(value, encryptionKey) {
    return crypto.AES.decrypt(value, encryptionKey).toString(CryptoJS.enc.Utf8);
}

export function set(key, value, useEncryption = false, encryptionKey = "none") {
    if (useEncryption == true) {
        value = crypto.AES.encrypt(value, encryptionKey).toString();
    }
    document.cookie = `${key}=${value}`;
}