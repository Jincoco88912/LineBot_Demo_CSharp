using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using isRock.LineBot;
namespace WebApplication3.Controllers
{
    public class LineBotController : isRock.LineBot. LineWebHookControllerBase
    {
        string channelAccessToken = "LebyKv5oBEj8QZciBdf02BicLaxG4lJY23y11rMlxesxJ+N4/1t0dftGjAx2BMVKJJtLpFYKc5MYnkigKIqQHFPEfL4hdbQ2sLXXIluxiPqrVpv" +
            "wWpsZWJjJ6YZeMoXSD6YRuY6y7y3l/OXEayjsGQdB04t89/1O/w1cDnyilFU=";
        [Route("api/SimpleWebHook")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
            string userid = "Ua2ff05684b2e2d6568c10976c15f300a";

            try
            {
                if (LineEvent == null) return Ok();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                //回覆訊息
                var responseMsg = "";
                if (LineEvent.type == "message")
                {
                    if (LineEvent.message.type == "text") //收到文字
                    {
                        if (LineEvent.message.text == "M1區域2")
                        {
                            responseMsg = "HELLO";
                        }
                        this.ReplyMessage(LineEvent.replyToken, responseMsg);
                        if (LineEvent.message.text == "delete")
                        {
                            responseMsg = DeleteMenu();
                        }
                        switch (LineEvent.message.text)
                        {
                            case "/上一頁":
                                SwitchMenuTo("快捷選單1", LineEvent);
                                break;
                            case "/下一頁":
                                SwitchMenuTo("快捷選單2", LineEvent);
                                break;
                                //default: //有選單時不可用回覆, 會有授權問題
                                //    responseMsg = "你說了:" + LineEvent.message.text;
                                //    this.ReplyMessage(LineEvent.replyToken, responseMsg);
                                //    break;
                        }
                    }
                    if (LineEvent.message.type == "sticker") //收到貼圖  //有選單時不可用回覆, 會有授權問題
                        this.ReplyMessage(LineEvent.replyToken, 1, 2);
                }
                //檢查用戶如果當前沒有任何選單，則嘗試綁定快捷選單1
                var currentMenu = isRock.LineBot.Utility.GetRichMenuIdOfUser(LineEvent.source.userId, channelAccessToken);
                if (currentMenu == null || string.IsNullOrEmpty(currentMenu.richMenuId))
                {
                    createMenu();
                    SetDefaultMenu();
                    currentMenu = isRock.LineBot.Utility.GetRichMenuIdOfUser(LineEvent.source.userId, channelAccessToken);
                }
                if (isRock.LineBot.Utility.GetRichMenu(currentMenu.richMenuId, channelAccessToken) == null)
                {
                    SetDefaultMenu();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                //如果發生錯誤，傳訊息給Admin
                this.ReplyMessage(LineEvent.source.userId, "發生錯誤:\n" + ex.Message);
                return Ok();
            }
        }

        private bool SwitchMenuTo(string MenuName, Event LineEvent)
        {
            //抓取所有選單
            var menus = isRock.LineBot.Utility.GetRichMenuList(channelAccessToken);
            //列舉每一個
            foreach (var item in menus.richmenus)
            {
                //如果選單名稱為 MenuName
                if (item.name == MenuName)
                {
                    isRock.LineBot.Utility.LinkRichMenuToUser(item.richMenuId, LineEvent.source.userId, channelAccessToken);
                    return true;
                }
            }
            return false;
        }
        string menuid1 = "";
        string menuid2 = "";
        protected void createMenu()
        {
            //動態建立選單
            //第一個選單
            var menu1 = new isRock.LineBot.RichMenu.RichMenuItem()
            { name = "快捷選單1", selected = true, chatBarText = "快捷選單1" };
            menu1.size.width = 2500; menu1.size.height = 1686;
            //第二個選單
            var menu2 = new isRock.LineBot.RichMenu.RichMenuItem()
            { name = "快捷選單2", selected = true, chatBarText = "快捷選單2" };
            menu2.size.width = 2500; menu2.size.height = 1686;
            //建立按鈕區域
            //menu寬=2500 高=1686
            //如為6塊區域 寬皆為833, 高為843
            //上方三塊 (1)x=0,y=0,(2)x=833, y=0, (3)x=1666, y=0
            //下方三塊 (1)x=0,y=843,(2)x=833, y=843, (3)x=1666, y=843
            isRock.LineBot.RichMenu.Area[] areaAry = new isRock.LineBot.RichMenu.Area[12];
            //menu1 area 0-5 menu2 area 7-11
            for (int i = 0; i < 2; i++) //X
            {
                for (int j = 0; j < 3; j++) //Y
                {
                    var area1 = new isRock.LineBot.RichMenu.Area();
                    area1.bounds.x = j * 833;
                    area1.bounds.y = i * 843;
                    area1.bounds.width = 833;
                    area1.bounds.height = 843;
                    if (i == 0)
                        areaAry[j] = area1;
                    else
                        areaAry[3 + j] = area1;
                    var area2 = new isRock.LineBot.RichMenu.Area();
                    menu1.areas.Add(area1);
                    area2.bounds.x = j * 833;
                    area2.bounds.y = i * 843;
                    area2.bounds.width = 833;
                    area2.bounds.height = 843;
                    if (i == 0)
                        areaAry[6 + j] = area2;
                    else
                        areaAry[9 + j] = area2;
                    menu2.areas.Add(area2);
                }
            }
            //按鈕行為
            //menu1
            areaAry[0].action = new isRock.LineBot.MessageAction() { label = "/edrftgyhju", text = "/Hello" };
            areaAry[1].action = new isRock.LineBot.MessageAction() { label = "/M1區域2", text = "/M1區域2" };
            areaAry[2].action = new isRock.LineBot.MessageAction() { label = "/M1區域3", text = "/M1區域3" };
            areaAry[3].action = new isRock.LineBot.MessageAction() { label = "/M1區域4", text = "/M1區域4" };
            areaAry[4].action = new isRock.LineBot.MessageAction() { label = "/M1區域5", text = "/M1區域5" };
            areaAry[5].action = new isRock.LineBot.MessageAction() { label = "/下一頁", text = "/下一頁" };
            //menu2
            areaAry[6].action = new isRock.LineBot.MessageAction() { label = "/上一頁", text = "/上一頁" };
            areaAry[7].action = new isRock.LineBot.MessageAction() { label = "/M2區域2", text = "/M2區域2" };
            areaAry[8].action = new isRock.LineBot.MessageAction() { label = "/M2區域3", text = "/M2區域3" };
            areaAry[9].action = new isRock.LineBot.MessageAction() { label = "/M2區域4", text = "/M2區域4" };
            areaAry[10].action = new isRock.LineBot.MessageAction() { label = "/M2區域5", text = "/M2區域5" };
            areaAry[11].action = new isRock.LineBot.MessageAction() { label = "/M2區域6", text = "/M2區域6" };

            var item = isRock.LineBot.Utility.CreateRichMenu(menu1, new Uri("https://i.imgur.com/HLv4R2T.jpg"), channelAccessToken);
            var item2 = isRock.LineBot.Utility.CreateRichMenu(menu2, new Uri("https://imgur.com/ukgGq2v.jpg"), channelAccessToken);
            string temp1 = "第1組richMenuId : " + item.richMenuId;
            menuid1 = item.richMenuId;
            string temp2 = "第2組richMenuId : " + item2.richMenuId;
            menuid2 = item2.richMenuId;
        }

        protected void createMenu2()
        {
            //動態建立選單
            var menu = new isRock.LineBot.RichMenu.RichMenuItem()
            { name = "快捷選單1", selected = true, chatBarText = "快捷選單1" };
            menu.size.width = 2500; menu.size.height = 1686;
            var area = new isRock.LineBot.RichMenu.Area();
            area.bounds.x = 1666;
            area.bounds.y = 843;
            area.bounds.width = 833;
            area.bounds.height = 843;
            //按鈕行為
            area.action = new isRock.LineBot.MessageAction() { label = "/下一頁", text = "/下一頁" };
            //加入
            menu.areas.Add(area);
            var item = isRock.LineBot.Utility.CreateRichMenu(menu, new Uri("https://i.imgur.com/HLv4R2T.jpg"), channelAccessToken);
            //Response.Write("<br/>第1組richMenuId : " + item.richMenuId);
            string temp1 = "第1組richMenuId : " + item.richMenuId;
            //ViewState["menuid"] = item.richMenuId;
            menuid1 = item.richMenuId;
            //第二個選單
            var menu2 = new isRock.LineBot.RichMenu.RichMenuItem()
            { name = "快捷選單2", selected = true, chatBarText = "快捷選單2" };
            menu.size.width = 2500; menu.size.height = 1686;
            //區域
            var area2 = new isRock.LineBot.RichMenu.Area();
            area2.bounds.x = 0;
            area2.bounds.y = 0;
            area2.bounds.width = 833;
            area2.bounds.height = 843;
            //行為
            area2.action = new isRock.LineBot.MessageAction() { label = "/上一頁", text = "/上一頁" };
            //加入
            menu2.areas.Add(area2);
            //建立選單
            var item2 = isRock.LineBot.Utility.CreateRichMenu(menu2, new Uri("https://imgur.com/ukgGq2v.jpg"), channelAccessToken);
            //Response.Write("<br/>第2組richMenuId : " + item2.richMenuId);
            string temp2 = "第2組richMenuId : " + item2.richMenuId;
            menuid2 = item2.richMenuId;
        }


        protected void SetDefaultMenu()
        {
            //var MenuId = ViewState["menuid"].ToString();
            //設定預設選單
            isRock.LineBot.Utility.SetDefaultRichMenu(menuid1, channelAccessToken);
            //Response.Write("done, now you can check your bot's rich menu.");
            string temp = "done, now you can check your bot's rich menu.";
        }

        protected string DeleteMenu()
        {
            string temp = "";
            //取消預設選單
            isRock.LineBot.Utility.CancelDefaultRichMenu(channelAccessToken);
            //抓取所有選單
            var menus = isRock.LineBot.Utility.GetRichMenuList(channelAccessToken);
            //顯示數量
            //Response.Write("Menu Count : " + menus.richmenus.Count());
            //刪除每一個
            foreach (var item in menus.richmenus)
            {
                isRock.LineBot.Utility.DeleteRichMenu(item.richMenuId, channelAccessToken);
                //Response.Write("<br/> deleted : " + item.richMenuId);
                temp += item.richMenuId + ",";
            }
            return temp;
        }
    
}
}
