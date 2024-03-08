namespace Grains
{
    [GenerateSerializer]
    [Immutable]
    public class TranslationItem : IEquatable<TranslationItem>
    {
        public TranslationItem(
               Guid id, Guid textId, Dictionary<Guid, string> languageText, Guid ownerId,
                  string createdBy, DateTime createdDate, string modifiedBy,
                  DateTime modifiedDate)
            : this(id, textId, languageText, ownerId, createdBy, createdDate,
                   modifiedBy, modifiedDate, DateTime.UtcNow)
        {
        }

        protected TranslationItem(
                  Guid id, Guid textId, Dictionary<Guid, string> languageText, Guid ownerId,
                  string createdBy, DateTime createdDate, string modifiedBy, DateTime modifiedDate, DateTime timeStamp)
        {
            Id = id;
            TextId = textId;
            LanguageText = languageText;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            ModifiedBy = modifiedBy;
            TimeStamp = timeStamp;
            OwnerId = ownerId;
        }

        [Id(0)]
        public Guid Id { get; }
        [Id(1)]
        public Guid TextId { get; }
        [Id(2)]
        public Dictionary<Guid, string> LanguageText { get; }
        [Id(3)]
        public string CreatedBy { get; }
        [Id(4)]
        public DateTime CreatedDate { get; }
        [Id(5)]
        public string ModifiedBy { get; }
        [Id(6)]
        public DateTime? ModifiedDate { get; }
        [Id(7)]
        public DateTime? TimeStamp { get; }
        [Id(8)]
        public Guid OwnerId { get; }

        public bool Equals(TranslationItem other)
        {
            if (other == null) return false;
            return Id == other.Id
                && TextId == other.TextId
                && LanguageText == other.LanguageText
                && CreatedDate == other.CreatedDate
                && CreatedBy == other.CreatedBy
                && ModifiedBy == other.ModifiedBy
                && ModifiedDate == other.ModifiedDate
                && OwnerId == other.OwnerId
                && TimeStamp == other.TimeStamp;
        }
    }
}