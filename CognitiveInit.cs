﻿using EPiServer.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Notification;

namespace Episerver.Labs.Cognitive
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CognitiveInit : IInitializableModule
    {
        protected Injected<IContentEvents> events { get; set; }
        protected Injected<IBlobFactory> blobfactory { get; set; }
        protected Injected<INotifier> notifier { get; set; }

        protected VisionHandler handler;

        public void Initialize(InitializationEngine context)
        {
            context.InitComplete += Context_InitComplete;
        }

        private void Context_InitComplete(object sender, EventArgs e)
        {
            handler = new VisionHandler();
            if (handler.Enabled)
            {
                events.Service.SavingContent += Service_SavingContent;
                
            }
        }

        private void Service_SavingContent(object sender, EPiServer.ContentEventArgs e)
        {
            
            //Only call when an image is being uploaded
            if(e.ContentLink.WorkID==0 && handler.Enabled && e.Content is ImageData && ((EPiServer.SaveContentEventArgs)e).Action==EPiServer.DataAccess.SaveAction.Publish)
            {
                var img = e.Content as ImageData;
                //Determine if Blob has changed / is new. If not, do nothing. Only act if relevant properties are empty/null

                handler.HandleImage(img);


            }
        }


        
        


        

        //            //Notify

        //            var msg = new NotificationMessage();
        //            msg.ChannelName = "ImageAnalysis";
        //            msg.TypeName = "ImageAnalysis";
        //            msg.Sender = new NotificationUser(EPiServer.Personalization.EPiServerProfile.Current.UserName);
        //            msg.Recipients = new[] { msg.Sender };
        //            msg.Subject = e.Content.Name;
        //            msg.Content = "You added an image showing " + res.Description.Captions.Select(c => c.Text).FirstOrDefault();
        //            notifier.Service.PostNotificationAsync(msg);
        //        }

        //    }
        //}

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
