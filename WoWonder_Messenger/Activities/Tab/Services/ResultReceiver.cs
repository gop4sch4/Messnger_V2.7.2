using Android.OS;
using Android.Runtime;
using System;

namespace WoWonder.Activities.Tab.Services
{
    public class ServiceResultReceiver : ResultReceiver
    {
        public IReceiver Receiver;

        public interface IReceiver
        {
            void OnReceiveResult(int resultCode, Bundle resultData);
        }

        public void SetReceiver(IReceiver receiver)
        {
            Receiver = receiver;
        }

        protected ServiceResultReceiver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ServiceResultReceiver(Handler handler) : base(handler)
        {

        }

        protected override void OnReceiveResult(int resultCode, Bundle resultData)
        {
            Receiver?.OnReceiveResult(resultCode, resultData);
        }
    }
}