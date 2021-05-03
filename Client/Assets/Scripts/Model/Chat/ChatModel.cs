using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Server.gRPC;
using Model.Base;
using Grpc.Core;

namespace Model.Chat
{
    public class ChatModel : ModelBase<ChatModel>
    {
        /// <summary>
        /// ���M����`���b�g���̋l�܂������N�G�X�g�L���[
        /// </summary>
        private Queue<DuplexChatSend> RequestSendMessageQueue = new Queue<DuplexChatSend>();

        /// <summary>
        /// �󂯎�����`���b�g���
        /// </summary>
        public List<DuplexChatReceive> ReceivedMessages = new List<DuplexChatReceive>();

        /// <summary>
        /// ���b�Z�[�W�\���̋��e��
        /// </summary>
        public uint MessageCapacity { get; private set; } = 5;

        /// <summary>
        /// �`���b�g���b�Z�[�W�̃��N�G�X�g���ɍs���R�[���o�b�N
        /// �����M�͕ʃX���b�h�ōs���AUnityAPI���g���Ȃ��̂ŃR�[���o�b�N�̃^�C�~���O�̓��N�G�X�g���ɂ��Ă�B
        /// </summary>
        public event Action<DuplexChatSend> OnRequestChatMessage = null;

        /// <summary>
        /// �`���b�g��M���ɍs���R�[���o�b�N
        /// </summary>
        public event Action<DuplexChatReceive> OnReciveChatMessage = null;

        /// <summary>
        /// �o�����X�g���[�~���O
        /// CL �� SV�`���b�g���b�Z�[�W���M����
        /// >>> S2C_Receive_Duplex_Chat
        /// </summary>
        /// <param name="receive"></param>
        //public void C2S_Send_Duplex_Chat(System.Threading.Tasks.Task ,DuplexChatSend send)
        public async Task C2S_Send_Duplex_Chat(AsyncDuplexStreamingCall<DuplexChatSend, DuplexChatReceive> call, Task receiveTask)
        {
            try
            {
                // ���N�G�X�g�̏�������
                foreach (var request in RequestSendMessageQueue)
                {
                    await call.RequestStream.WriteAsync(request);
                }
                RequestSendMessageQueue.Clear();

                // �������ݏI���܂ő҂�
                await call.RequestStream.CompleteAsync();

                // ��M�������I���܂ő҂�
                await receiveTask;
            }
            catch (Exception e)
            {
                Sample.ClientSample.context.Post(_ =>
                {
                    Debug.LogError($"<color=red>C2S_Send_Chat</color>:{e.GetType()}\n{e.Message}\n{e.StackTrace}");
                }, null);
                throw;
            }
        }

        /// <summary>
        /// �o�����X�g���[�~���O
        /// SV �� CL�`���b�g���b�Z�[�W��M����
        /// >>> C2S_Send_Duplex_Chat
        /// </summary>
        /// <param name="receive"></param>
        public void S2C_Receive_Duplex_Chat(DuplexChatReceive receive)
        {
            // �����f�[�^���d�����ēo�^����邱�Ƃ�h�����߂ɑI��
            if (!ReceivedMessages.Any(_ => _.Hash == receive.Hash))
            {
                // �󂯎�菈��
                OnReciveChatMessage?.Invoke(receive);
                ReceivedMessages.Add(receive);
            }
        }

        /// <summary>
        /// ���M���b�Z�[�W�̃��N�G�X�g
        /// </summary>
        /// <param name="request"></param>
        public void AddRequestSendMessage(DuplexChatSend request)
        {
            // ���N�G�X�g����
            OnRequestChatMessage?.Invoke(request);
            RequestSendMessageQueue.Enqueue(request);
        }
    }
}