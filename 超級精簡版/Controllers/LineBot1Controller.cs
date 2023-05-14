using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace linebot_test2.Controllers
{
    public class LineBot1Controller : isRock.LineBot.LineWebHookControllerBase
    {
        [Route("api/SimpleWebHook")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            string userid = "Uddfe5fcab3a48cb3bf4e51dd571c7a72";
            try
            {
                this.ChannelAccessToken = "jjGDt1g7VaMQLEYIDQLUlKRkl6noXjp86lptuOd3WBu67hS6KenWHdja7VBBS0qPxDFERp2lerNrjncF512MCDFHU3KIsKTUl75jl4LbXUffoG7QnlUZ3GSiZNJqVb3gDCMTgro1w7TsZMUkSsmABgdB04t89/1O/w1cDnyilFU";
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                var responseMsg = "";
                var sendUid = ReceivedMessage.events[0].source.userId;
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    responseMsg = $"你剛說: {LineEvent.message.text}";
                }                       
                else
                {
                    responseMsg = $"收到 event : {LineEvent.type}";
                }
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                return Ok();
            }
            catch (Exception ex)
            {
                this.PushMessage(userid, "發生錯誤:\n" + ex.Message);
                return Ok();
            }
        }
    }
}