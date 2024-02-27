namespace Grains
{
    [GenerateSerializer]
    [Immutable]
    public class BaseModel
    {
        [Id(0)]
        public bool IsDirty { get; protected set; }

        protected void SetProperty<T>(ref T property, T value)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
            {
                return;
            }

            property = value;
            IsDirty = true;
        }

        public void ClearDirty()
        {
            IsDirty = false;
        }
    }
}