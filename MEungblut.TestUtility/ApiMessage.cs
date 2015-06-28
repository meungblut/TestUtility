namespace MEungblut.TestUtility
{
    using System.Net.Http;
    using System.Text;

    using Newtonsoft.Json;

    public abstract class ApiMessage
    {
        protected ApiMessage()
        {
        }

        public abstract string UrlFragmentToSendTo { get; protected set; }

        public abstract object MessageToSerializeToBody { get; protected set; }

        public abstract HttpVerb HttpVerb { get; }

        public string BodyAsString()
        {
            return JsonConvert.SerializeObject(this.MessageToSerializeToBody);
        }

        public StringContent BodyAsStringContent()
        { 
            return new StringContent(this.BodyAsString(), Encoding.UTF8, "application/json");
        }
    } 
}
