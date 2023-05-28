using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Tozawa.Bff.Portal.extension;

namespace Tozawa.Bff.Portal.Models.Enums
{
    public enum FunctionType
    {
        [EnumMember]
        [Description("e862a9be-2272-4b47-8a03-8a2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Read,
            PairId = "9d49e96e-d0bf-453b-ba61-36b21181693b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        ReadMember = 0,

        [EnumMember]
        [Description("e862a9be-2272-4b47-8a03-8a2ef564029d")]
        [FunctionTypeValues(
            AccessType = AccessType.Write,
            PairId = "9d49e96e-d0bf-453b-ba61-36b21161693c", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        WriteMember = 1,

        [EnumMember]
        [Description("e862a9be-2272-4b47-8a03-8a2ef5658292")]
        [FunctionTypeValues(
            AccessType = AccessType.Read,
            PairId = "9d49e96e-d0bf-453b-ba61-36c21181893b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        ReadActivity = 2,

        [EnumMember]
        [Description("e862a9be-2272-4b47-7a03-8a2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Write,
            PairId = "9d49e96e-d0bf-453b-ba61-36b21181694j", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        WriteActivity = 3,

        [EnumMember]
        [Description("e862t9be-2272-4b47-8a03-8a2ef5850292")]
        [FunctionTypeValues(
            AccessType = AccessType.Read,
            PairId = "9d49e96e-d0bf-453b-ba91-36b21186693b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        ReadSevices = 4,

        [EnumMember]
        [Description("e862a9fe-2272-4b47-8a03-5a2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Write,
            PairId = "9d49d96e-d0bf-453b-ba61-39b21181673b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        WriteServices = 5,

        [EnumMember]
        [Description("e862b9be-2272-4b47-8a03-8a2eh5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Read,
            PairId = "9d49h96e-d0bf-453b-ba61-36b27181693b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        ReadPayment = 6,

        [EnumMember]
        [Description("e861a9be-2272-4b47-8a03-8r2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Write,
            PairId = "9d49e96e-d0tf-453b-ba61-36b23181693b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        WritePayment = 7,

        [EnumMember]
        [Description("e862a9ge-2722-4b47-8a03-8a2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Read,
            PairId = "9d49e96e-d0bf-453b-ba61-36b21181690b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        ReadLanguageorization = 8,

        [EnumMember]
        [Description("e862a7be-2272-4b47-8u03-8a2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Write,
            PairId = "9d49e96e-d0bf-453b-ba61-36b21101693b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        WriteLanguageorization = 9,

        [EnumMember]
        [Description("e862a9je-2722-4b47-8a03-8a2ef4650292")]
        [FunctionTypeValues(
          AccessType = AccessType.Read,
          PairId = "9d49e96e-d0bk-453b-ba61-36b21181590b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        ReadLanguage = 10,

        [EnumMember]
        [Description("e862a7bf-2272-4b47-8u03-8a2ef5659292")]
        [FunctionTypeValues(
            AccessType = AccessType.Write,
            PairId = "9d49e96e-d0bh-453b-ba61-36b21101673b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        WriteLanguage = 11,

        [EnumMember]
        [Description("r862a9ge-2722-4c47-8a03-8a2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Read,
            PairId = "8d49e96e-d0bf-453b-ba61-76b21181690b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        ReadSetting = 12,

        [EnumMember]
        [Description("t862a7be-2272-4b47-9u03-8a2ef5650292")]
        [FunctionTypeValues(
            AccessType = AccessType.Write,
            PairId = "9d49e96e-d1bf-453b-ba61-36b21111693b", OnlyVisibleForRoot = false)]
        [JsonConverter(typeof(FunctionTypeConverter))]
        WriteSetting = 13,
    }
}


