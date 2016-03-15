using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace TvBand.Uwp.Services
{
    public interface IObservableRestClient
    {
        IObservable<T> Get<T>(Uri uri);

        IObservable<Unit> Post<T>(Uri uri, T value);
    }

    internal class ObservableRestClient : IObservableRestClient
    {
        private static readonly JsonSerializer _serializer = new JsonSerializer();

        private readonly IScheduler _scheduler;
        private readonly Uri _baseAddress;

        public ObservableRestClient(Uri baseAddress, IScheduler scheduler)
        {
            _scheduler = scheduler ?? new EventLoopScheduler();

            _baseAddress = baseAddress;
        }

        private static HttpContent ContentFromHttpResponseMessage(HttpResponseMessage source)
        {
            source.EnsureSuccessStatusCode(); // Will throw if request wasn't processed successfully

            return source.Content;
        }

        private static async Task<T> DeserializeContent<T>(HttpContent content)
        {
            Stream stream = await content.ReadAsStreamAsync();

            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (JsonReader jsonReader = new JsonTextReader(streamReader))
                {
                    return _serializer.Deserialize<T>(jsonReader);
                }
            }
        }

        public IObservable<T> Get<T>(Uri resource)
        {
            return Observable.Create<T>(
                observer =>
                {
                    HttpClient client = new HttpClient();

                    client.BaseAddress = _baseAddress;
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    SerialDisposable request = new SerialDisposable();
                    request.Disposable = Disposable.Empty;

                    Uri requestUri;
                    if (Uri.TryCreate(_baseAddress, resource, out requestUri))
                    {
                        request.Disposable = Observable
                            .FromAsync(() => client.GetAsync(requestUri))
                            .Select(ContentFromHttpResponseMessage)
                            .SelectMany(content => DeserializeContent<T>(content))
                            .SubscribeOn(_scheduler)
                            .Subscribe(observer);
                    }
                    else
                    {
                        observer.OnError(
                            new InvalidOperationException($"Could not create an absolute address from the base Uri '{_baseAddress.ToString()}' and the resource Uri '{resource.ToString()}'")
                        );
                    }

                    return new CompositeDisposable(
                        request,
                        client
                    );
                }
            );
        }
        
        public IObservable<Unit> Post<T>(Uri uri, T value)
        {
            throw new NotImplementedException();
        }
    }
}
