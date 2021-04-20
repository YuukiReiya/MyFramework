//.NETコレクション各種:https://qiita.com/4_mio_11/items/8daf0e391642363e795e
//コレクション計算量:https://qiita.com/takutoy/items/37e81b916271bf43b527
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Model;

namespace Chat
{
    partial class ChatModel : AbstractModel<ChatModel>, IModel
    {
        public class Info
        {
            public Info(int id, string message)
            {
                UserID = id;
                Message = message;
            }
            public int UserID;
            public string Message;
        }

        public Queue<Info> Messages = new Queue<Info>((int)MessageCapacity);
        public const uint MessageCapacity= 50;
    }
}
