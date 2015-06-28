namespace MEungblut.TestUtility
{
    interface IHttpCommunicator
    {
        System.Threading.Tasks.Task<HttpResponse> Get(HttpRequest message);
        System.Threading.Tasks.Task<HttpResponse> Post(HttpRequest message);
    }
}
