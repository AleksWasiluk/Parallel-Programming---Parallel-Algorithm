using System;
using System.Collections.Generic;
using System.Text;

namespace lab03
{
    class MessagesManager
    {
        List<object> ListLocker = new List<object>();
        List<Queue<Message>> listQueueMessage = new List<Queue<Message>>();

        public MessagesManager()
        {
            for(int i =0;i<Const.n;i++)
            {
                ListLocker.Add(new object());
                listQueueMessage.Add(new Queue<Message>());
            }
        }

        public void Send(int index, Message message)
        {
            lock(ListLocker[index])
            {
                listQueueMessage[index].Enqueue(message);
            }     
        }

        public Message Receive(int index)
        {
            Message result = null;
            lock (ListLocker[index])
            {
                if(listQueueMessage[index].Count != 0)
                    result = listQueueMessage[index].Dequeue();
            }
            return result;
        }

    }
    abstract class Message
    {
        public StatesMessage statesMessage;
        public string message;
        public int numberRound;
        public Message( string _message, int _numberRound,StatesMessage _statesMessage)
        {
            statesMessage = _statesMessage;
            message = _message;
            numberRound = _numberRound;
        }
    }
    class ExchangeNumbersMessage : Message
    {
        public ExchangeNumbersMessage(string _message,int _number, int _index,int _numberRound, StatesMessage _statesMessage) : base(_message, _numberRound, _statesMessage)
        { index = _index; number = _number; }
        int index,number;
        public int Index { get { return index; } }
        public int Number { get { return number; } }
    }

    class NullMessage : Message
    {
        public NullMessage(int _numberRound,StatesMessage _statesMessage) : base("",_numberRound, _statesMessage)
        { }
       
    }
    class FinishConcertMessage : Message
    {
        private int index;
        public int Index { get { return index; } }
        public FinishConcertMessage(string message, int _indexFinishedJankiel,int _numberRound) :base(message, _numberRound, StatesMessage.Listening)
        {
            index = _indexFinishedJankiel;
        }
    }


    class NotThreatenToBeLeaderMessage : Message
    {
        public NotThreatenToBeLeaderMessage(string _message, int _indexNotThreaten,int _numberRound,StatesMessage _statesMessage) : base(_message, _numberRound, _statesMessage)
        { indexNotThreaten = _indexNotThreaten; }
        int indexNotThreaten;
        public int IndexNotThreaten { get { return indexNotThreaten; } }
    }


    class LeaderMessage : Message
    {
        public LeaderMessage(string _message,int _indexLeader, int _numberRound, StatesMessage _statesMessage) :base(_message, _numberRound, _statesMessage)
        { indexLeader = _indexLeader; }
        int indexLeader;
        public int IndexLeader { get { return indexLeader; } }
    }
     
}
