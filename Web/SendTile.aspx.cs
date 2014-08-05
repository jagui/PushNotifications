using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace toasts
{
    public partial class SendTile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonSendTile_Click(object sender, EventArgs e)
        {


            try
            {
                // Get the URI that the Microsoft Push Notification Service returns to the Push Client when creating a notification channel.
                // Normally, a web service would listen for URIs coming from the web client and maintain a list of URIs to send
                // notifications out to.
                string subscriptionUri = TextBoxUri.Text.ToString();


                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(subscriptionUri);

                // Create an HTTPWebRequest that posts the Tile notification to the Microsoft Push Notification Service.
                // HTTP POST is the only method allowed to send the notification.
                sendNotificationRequest.Method = "POST";

                // The optional custom header X-MessageID uniquely identifies a notification message. 
                // If it is present, the same value is returned in the notification response. It must be a string that contains a UUID.
                // sendNotificationRequest.Headers.Add("X-MessageID", "<UUID>");

                // Create the Tile message.
                string tileMessage = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<wp:Notification xmlns:wp=\"WPNotification\">" +
                    "<wp:Tile>" +
                      "<wp:BackgroundImage>" + TextBoxBackgroundImage.Text + "</wp:BackgroundImage>" +
                      "<wp:Count>" + TextBoxCount.Text + "</wp:Count>" +
                      "<wp:Title>" + TextBoxTitle.Text + "</wp:Title>" +
                      "<wp:BackBackgroundImage>" + TextBoxBackBackgroundImage.Text + "</wp:BackBackgroundImage>" +
                      "<wp:BackTitle>" + TextBoxBackTitle.Text + "</wp:BackTitle>" +
                      "<wp:BackContent>" + TextBoxBackContent.Text + "</wp:BackContent>" +
                   "</wp:Tile> " +
                "</wp:Notification>";

                // Set the notification payload to send.
                byte[] notificationMessage = Encoding.Default.GetBytes(tileMessage);

                // Set the web request content length.
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers.Add("X-WindowsPhone-Target", "token");
                sendNotificationRequest.Headers.Add("X-NotificationClass", "1");


                using (Stream requestStream = sendNotificationRequest.GetRequestStream())
                {
                    requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                }

                // Send the notification and get the response.
                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.GetResponse();
                string notificationStatus = response.Headers["X-NotificationStatus"];
                string notificationChannelStatus = response.Headers["X-SubscriptionStatus"];
                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                // Display the response from the Microsoft Push Notification Service.  
                // Normally, error handling code would be here. In the real world, because data connections are not always available,
                // notifications may need to be throttled back if the device cannot be reached.
                TextBoxResponse.Text = notificationStatus + " | " + deviceConnectionStatus + " | " + notificationChannelStatus;
            }
            catch (Exception ex)
            {
                TextBoxResponse.Text = "Exception caught sending update: " + ex.ToString();
            }

        }
    }
}