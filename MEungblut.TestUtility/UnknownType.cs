namespace MEungblut.TestUtility
{
    public class UnknownType
    {
        public UnknownType(string unknownType)
        {
            this.DataForUnknownType = unknownType;
        }

        public string DataForUnknownType { get; private set; }
    }
}
