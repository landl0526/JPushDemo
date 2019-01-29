using System;
using System.IO;
using System.Linq;
using Foundation;
using UIKit;
using UserNotifications;

namespace MyTestExtension
{
    [Register("NotificationService")]
    public class NotificationService : UNNotificationServiceExtension
    {
        Action<UNNotificationContent> ContentHandler { get; set; }
        UNMutableNotificationContent BestAttemptContent { get; set; }

        protected NotificationService(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            ContentHandler = contentHandler;
            BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

            // Modify the notification content here...
            BestAttemptContent.Title = @"Luting";
            //this.bestAttemptContent.subtitle = @"";
            //this.bestAttemptContent.body = @"";

            // Set the attachment
            //NSDictionary dict = BestAttemptContent.UserInfo;
            //NSString imgUrl = dict["imageAbsoluteString"] as NSString;

            //if (imgUrl.Length == null)
            //{
            //    ContentHandler(BestAttemptContent);
            //}
            //loadAttachmentForUrlString(imgUrl, "png", (attach) =>
            //{
            //    if (attach != null)
            //    {
            //        BestAttemptContent.Attachments = new UNNotificationAttachment[] { attach };
            //    }
            //    ContentHandler(BestAttemptContent);
            //});


            string suiteName = "group.com.companyname.ExtensionDemo";
            var appGroupContainerUrl = NSFileManager.DefaultManager.GetContainerUrl(suiteName);
            var directoryNameInAppGroupContainer = Path.Combine(appGroupContainerUrl.Path, "Pictures");

            var filenameDestPath = Path.Combine(directoryNameInAppGroupContainer, "MyPic.png");

            var localDoc = NSSearchPath.GetDirectories(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).FirstOrDefault();
            var localPath = Path.Combine(localDoc, "MyPic.png");
            NSError error = null;
            NSFileManager.DefaultManager.Copy(filenameDestPath, localPath, out error);

            NSError attachmentError = null;
            UNNotificationAttachment attachment = UNNotificationAttachment.FromIdentifier("", new NSUrl(localPath, false), options: null, error: out attachmentError);
            BestAttemptContent.Attachments = new UNNotificationAttachment[] { attachment };
            contentHandler(BestAttemptContent);
        }

        delegate void CompletionHandler(UNNotificationAttachment attach);
        void loadAttachmentForUrlString(string urlString, string type, CompletionHandler completionHandler)
        {
            NSUrl attachmentURL = new NSUrl(urlString);

            NSUrlSession session = NSUrlSession.FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration);
            session.CreateDownloadTask(attachmentURL, (url, response, error) => {

                if (error != null)
                {
                    // Fail
                }
                else
                {
                    NSFileManager fileManager = NSFileManager.DefaultManager;
                    NSUrl localUrl = new NSUrl(url.Path + "." + type, false);
                    NSError moveError = null;
                    fileManager.Move(url, localUrl, out moveError);

                    NSError attachmentError = null;
                    UNNotificationAttachment attachment = UNNotificationAttachment.FromIdentifier("", localUrl, options: null, error: out attachmentError);
                    if (attachmentError != null)
                    {
                        // Fail
                    }
                    else
                    {
                        completionHandler(attachment);
                    }
                }
            }).Resume();
        }

        public override void TimeWillExpire()
        {
            // Called just before the extension will be terminated by the system.
            // Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

            ContentHandler(BestAttemptContent);
        }
    }
}
