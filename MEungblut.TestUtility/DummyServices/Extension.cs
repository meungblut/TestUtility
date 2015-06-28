namespace MEungblut.TestUtility.DummyServices
{
    using System.IO;

    using Nancy.IO;

    static class Extension
    {
        public static string ReadAsString(this RequestStream requestStream)
        {
            using (var reader = new StreamReader(requestStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}