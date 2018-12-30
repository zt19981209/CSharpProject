using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NetDiskClient.utils
{
    /// <summary>
    /// 存放所有来自客户端的消息
    /// </summary>
    internal class MessagePool
    {
        private MessagePool()
        {
        }

        private static MessagePool messagePool = new MessagePool();

        public static MessagePool GetMessagePool()
        {
            return messagePool;
        }

        private static Dictionary<string, List<Msg>> msgDic = new Dictionary<string, List<Msg>>();

        //添加所有消息
        public void AddMessage(string json_str)
        {
            JObject jo = JObject.Parse(json_str);
            if (jo["type"].ToString() != "msg")
            {
                return;
            }
            //把接收到的消息包装在Msg对象里
            Msg msg = new Msg();
            msg.type = jo["type"].ToString();
            msg.toUid = jo["toUid"].ToString();
            msg.icon = jo["icon"].ToString();
            msg.nickname = jo["nickname"].ToString();
            msg.myUid = jo["myUid"].ToString();
            msg.msg = jo["msg"].ToString();
            msg.code = jo["code"].ToString();

            //链表集合存Msg对象
            List<Msg> list = null;
            try
            {
                list = msgDic[msg.myUid];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                list = new List<Msg>();
                msgDic.Add(msg.myUid, list);
            }
            list.Add(msg);
            msgDic[msg.myUid] = list;

            Config.mainWindow.AddMessageF(msg);
        }
    }
}