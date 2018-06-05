using System;
using System.Collections.Generic;
using System.Threading;

namespace lab03
{
    enum StatesMessage
    {
        ChangeNumbers,SendingLeaders,Listening,Playing
    }
    class Jankiel
    {


        StatesMessage statesMessage;
        static private MessagesManager messagesManager = new MessagesManager();

        private List<int> listNeighboursNumbers = new List<int>();
        private List<int> listNeigbour = new List<int>();
        private List<int> listNeigbourThreaten = new List<int>();
        private Location location;

        private int numberRound;

        private int numberJankiel;
        private int indexJankiel;

        private Thread thread;

        public Jankiel(int index,List<Location> listLocation)
        {
            numberRound=0;
            statesMessage = StatesMessage.ChangeNumbers;

            indexJankiel = index;
            numberJankiel = indexJankiel;
            location = listLocation[indexJankiel];

            SearchForNeighboursList(listLocation);


            thread = new Thread(() => Thread_DoWork());
        }

        private void SendMessageNumberToNeigbours()
        {
            foreach (var i in listNeigbour)
            {
                Console.WriteLine($"Sending to {i} Jankiel From {indexJankiel} (Number)");
                var message = new ExchangeNumbersMessage($"Greetings {i} Jankiel From {indexJankiel}", indexJankiel, indexJankiel,numberRound, StatesMessage.ChangeNumbers);
                messagesManager.Send(i, message);
            }
        }

        bool gotLeaderMessage = false;
        bool isLeader = false;
        private void Thread_DoWork()
        {
            DoAlgorithm();
        }

        private void DoAlgorithm()
        {
            SendMessageNumberToNeigbours();
            ReceiveMessagesNumber();


            
            while (!isLeader)
            {
                
                statesMessage = StatesMessage.SendingLeaders;
                wasLeaderMessage = false;
                countLeaderNeigbours = 0;

                isLeader = CheckIfIsLeader();
                if (isLeader)
                {
                    SendLeaderMessage();
                    ReceiveMessageToWaitForOthersToFinishRound();
                }
                else
                {
                    ReceiveMessages();
                }

                if(isLeader)
                {
                    statesMessage = StatesMessage.Playing;
                    PlayConcert();
                }
                else
                {
                    statesMessage = StatesMessage.Listening;
                    WaitForFinishRoundMessageFromLeaders();
                }


                Console.WriteLine($"Jankiel {indexJankiel} isLeader = {isLeader} round = {numberRound}");
                numberRound++;
            }
            Console.WriteLine($"Jankiel {indexJankiel} has Finished!");
           
           
        }

        private void WaitForFinishRoundMessageFromLeaders()
        {
            while (countLeaderNeigbours != 0)
            {
                var message = ReceiveMessage();
                ReceivedFinishedMessage(message);
            }
        }

        private void ReceivedFinishedMessage(Message message)
        {
            if(message is FinishConcertMessage)
            {
                var finishedMessage = message as FinishConcertMessage;
                Console.WriteLine($"Jankiel {indexJankiel} received FinishedMessage round = {numberRound}");
                RemoveNeighbourFromList(finishedMessage.Index);
                countLeaderNeigbours--;
            }
            

        }

        private void PlayConcert()
        {
            Console.WriteLine($"Jankiel {indexJankiel} plays concert round = {numberRound}");
            Thread.Sleep(Const.timeConcert);
            SendFinishedConcertMessages();
        }

        private void SendFinishedConcertMessages()
        {
            foreach (var i in listNeigbour)
            {
                Console.WriteLine($"Sending to {i} Jankiel From {indexJankiel} (Finish)");
                var message = new FinishConcertMessage($"Finished Concert {i} Jankiel From Leader {indexJankiel}", indexJankiel, numberRound);
                messagesManager.Send(i, message);
            }
        }

        private void ReceiveMessageToWaitForOthersToFinishRound()
        {
            int neigboursRemaining = listNeigbour.Count;

            bool endloop = false;
            while (neigboursRemaining != 0)
            {
                var message = ReceiveMessage();
                SwitchMessageAndDoNotRespond(message, ref neigboursRemaining);
            }
        }

        private void SwitchMessageAndDoNotRespond(Message message, ref int neigboursRemaining)
        {
            if (message is NotThreatenToBeLeaderMessage)
            {
                Console.WriteLine($"{message.message} received ( {indexJankiel} )");
                neigboursRemaining--;

            }
            else//null message
            {
                Console.WriteLine($"empty message received ( {indexJankiel} )");
                neigboursRemaining--;
            }
        }

        private void SendNotThreatenMessages()
        {
            foreach (var i in listNeigbour)
            {
                if(i<numberJankiel)
                {
                    Console.WriteLine($"Sending to {i} Jankiel From {indexJankiel} (NotThreaten)");
                    var messageString = $"Greetings {i} Jankiel From notLeader {indexJankiel}";
                    var message = new NotThreatenToBeLeaderMessage(messageString, indexJankiel,numberRound, StatesMessage.SendingLeaders);
                    messagesManager.Send(i, message);
                }
                else
                {
                    Console.WriteLine($"Sending to {i} Jankiel From {indexJankiel} (NULL)");
                    var message = new NullMessage(numberRound, StatesMessage.SendingLeaders);
                    messagesManager.Send(i, message);
                }
               
            }
        }

        private void ReceiveMessages()
        {
            int neigboursRemaining = listNeigbour.Count;
            while (neigboursRemaining != 0)
            {
                var message = ReceiveMessage();                    
                SwitchMessageAndDecide(message,ref neigboursRemaining);
            }
        }

        private void ReceiveMessagesNumber()
        {
            int neigboursRemaining = listNeigbour.Count;
            bool[] receivedMessageNumberFromNeighbour = new bool[listNeigbour.Count];

            bool endloop = false;
            while (neigboursRemaining != 0)
            {
                var message = ReceiveMessage();
                ReceivedMessageNumber(message, ref neigboursRemaining);
            }



        }
        Message ReceiveMessage()
        {
            Message message = null;
            bool isOkMessage = false;
            while(!isOkMessage)
            {

                message = messagesManager.Receive(indexJankiel);

                if(message !=null)
                {
                    if (message.numberRound == numberRound && statesMessage == message.statesMessage)
                        isOkMessage = true;
                    else
                        messagesManager.Send(indexJankiel, message);//sendback message to end of queue
                }
                
                if(!isOkMessage)
                    Thread.Sleep(Const.timeIntervalMessage);
            }

            return message;
        }
        private void SendLeaderMessage()
        {
            foreach (var i in listNeigbour)
            {
                Console.WriteLine($"Sending to {i} Jankiel From {indexJankiel} (Leader)");
                var message = new LeaderMessage($"Greetings {i} Jankiel From Leader {indexJankiel}", indexJankiel,numberRound, StatesMessage.SendingLeaders);
                messagesManager.Send(i, message);
            }
        }

        private bool CheckIfIsLeader()
        {
            listNeigbourThreaten.Clear();
            bool result = isLeader;
            bool isSmaller = false;
            for(int i=0;i<listNeighboursNumbers.Count;i++)
            {
                if (numberJankiel < listNeighboursNumbers[i])
                {
                    isSmaller = true;
                    listNeigbourThreaten.Add(listNeigbour[i]);
                }
                    
            }

            if (isSmaller)
                result = false;
            else
                result = true;

            return result;
        }

        int countLeaderNeigbours = 0;
        bool wasLeaderMessage = false;
        private void SwitchMessageAndDecide(Message message,ref int neigboursRemaining)
        {
            if (message is LeaderMessage)
            {
                var leaderMessage = message as LeaderMessage;
                Console.WriteLine($"{message.message} received ( {indexJankiel} )");
                gotLeaderMessage = true;
                isLeader = false;
                if(!wasLeaderMessage)
                {
                    SendNotThreatenMessages();
                    wasLeaderMessage = true;
                }
                
                neigboursRemaining--;
                countLeaderNeigbours++;
            }
            else if (message is NotThreatenToBeLeaderMessage)
            {
                Console.WriteLine($"{message.message} received ( {indexJankiel} )");
                var notThreatenMessage = message as NotThreatenToBeLeaderMessage;

                RemoveItemFromThreatenList(notThreatenMessage.IndexNotThreaten);
                if (listNeigbourThreaten.Count == 0)
                {
                    isLeader = true;
                    SendLeaderMessage();
                }
                neigboursRemaining--;

            }
            else if(message is NullMessage)
            {
                Console.WriteLine($"empty message received ( {indexJankiel} )");
                neigboursRemaining--;
            }
        }

        private void RemoveNeighbourFromList(int indexLeader)
        {
           int indexListToRemove = -1;
           for(int i=0;i< listNeigbour.Count;i++)
           {
                if (listNeigbour[i] == indexLeader)
                {
                    indexListToRemove = i;
                    break;
                }
                    
           }
            listNeigbour.RemoveAt(indexListToRemove);
            listNeighboursNumbers.RemoveAt(indexListToRemove);
        }

        

        private void ReceivedMessageNumber(Message message,ref int neigboursRemaining)
        {
            
                var exchangeMessage = message as ExchangeNumbersMessage;
                int indexArray = FindIndexInNeighboursList(exchangeMessage.Index);

                Console.WriteLine($"{message.message} received ( {indexJankiel} )");

                listNeighboursNumbers[indexArray] = exchangeMessage.Number;

                neigboursRemaining--;
           
        }
        void RemoveItemFromThreatenList(int indexNotThreaten)
        {
            int index = -1;
            for (int i = 0; i < listNeigbourThreaten.Count; i++)
            {
                if (listNeigbourThreaten[i] == indexNotThreaten)
                    index = i;
            }
            listNeigbourThreaten.RemoveAt(index);
        }

        private int FindIndexInNeighboursList(int index)
        {
            int result = -1;
            for(int i=0;i< listNeigbour.Count;i++)
            {
                if(listNeigbour[i] == index)
                {
                    result = i;
                }
            }
            return result;
        }

        public void Init()
        {
            thread.Start();
        }

        private void SearchForNeighboursList(List<Location> listLocation)
        {
            for (int i = 0; i < listLocation.Count; i++)
            {
                if (i == indexJankiel)
                    continue;
                if (Location.Distance(location, listLocation[i]) < Const.MaxDist)
                {
                    listNeigbour.Add(i);    
                }
            }
            for(int i=0;i<listNeigbour.Count;i++)
            listNeighboursNumbers.Add(-1);
        }
        


    }
}
