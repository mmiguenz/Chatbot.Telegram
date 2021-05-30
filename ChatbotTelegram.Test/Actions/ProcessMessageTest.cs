using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatbotTelegram.Actions;
using ChatbotTelegram.Model;
using ChatbotTelegram.Services;
using Xunit;
using Moq;

namespace ChatbotTelegram.Test.Actions
{
    public class ProcessMessageTest
    {
        private readonly ProcessMessage _processMessage;
        private readonly Mock<IOneMenuService> _oneMenuService;
        private readonly Mock<IConversationStateService> _conversationState;

        public ProcessMessageTest()
        {
            _oneMenuService = new Mock<IOneMenuService>();
            _conversationState = new Mock<IConversationStateService>();
            _processMessage = new ProcessMessage(_oneMenuService.Object, _conversationState.Object);
        }
        
        [Fact]
        public async Task When_TransactionId_IsNot_Set_Returns_AvailableMenus()
        {
            var message = "a message";
            var chatId = 1L;
            
            SetConversationState(chatId, string.Empty);
            SetOneMenu_GetAllMenus();

            var proceessMessageResult = await _processMessage.Execute(chatId, message);

            _conversationState.Verify();
            _oneMenuService.Verify();
            
            Assert.NotNull(proceessMessageResult);
            Assert.NotEmpty(proceessMessageResult.AvailableMenus);
            Assert.Null(proceessMessageResult.CurrentStep);
        }

        [Fact]
        public async Task When_Receives_A_MenuCommand_Inits_MenuTransaction_And_Resets_ConversationState_And_Returns_First_Step()
        {
            var menu1 = Data.Menu1;
            var commandMenuMessage = $"/{menu1.Label}";
            var chatId = 1L;
            var newTransactionId = "transactionId000";

            SetInitMenuTransaction(menu1.Label, newTransactionId);
            SetGetCurrentStep(newTransactionId, Data.step1);
            SetConversationStateReset(chatId);
            SetConversationStateCurrentTransactionId(chatId, newTransactionId);

            var processMessageResult = await _processMessage.Execute(chatId, commandMenuMessage);

            _conversationState.Verify();
            _oneMenuService.Verify();
            
            Assert.NotNull(processMessageResult);
            Assert.Null(processMessageResult.AvailableMenus);
            Assert.NotNull(processMessageResult.CurrentStep);
            Assert.Equal(Data.step1.Text, processMessageResult.CurrentStep.Text);
        }

        [Fact]
        public async Task When_Receives_A_response_ForA_Step_And_Success_Returns_NextStep()
        {
            var chatId = 1L;
            var transactionId = "transactionId000";
            var message = "step 2 answer";

            var expectedProcessMessageResult = new ProcessMessageResult()
            {
                CurrentStep = Data.step2
            };

            SetOneMenuGetCurrentMenuTransactionId(chatId, transactionId);
            SetOneMenuSaveResponse(transactionId, message, expectedProcessMessageResult);

            var processMessageResult = await _processMessage.Execute(chatId, message);

            _conversationState.Verify();
            _oneMenuService.Verify();
            
            Assert.NotNull(processMessageResult);
            Assert.Null(processMessageResult.AvailableMenus);
            Assert.Null(processMessageResult.Errors);
            Assert.NotNull(processMessageResult.CurrentStep);
            Assert.Equal(Data.step2.Text, processMessageResult.CurrentStep.Text);
        }

        [Fact]
        public async Task When_Receives_A_response_ForA_Step_And_HasErrors_Returns_SameStep()
        {
            var chatId = 1L;
            var transactionId = "transactionId000";
            var message = "step 2 answer";

            var expectedProcessMessageResult = new ProcessMessageResult()
            {
                CurrentStep = Data.step1,
                Errors = new List<string>() {"invalid data"},
                HasErrors = true

            };

            SetOneMenuGetCurrentMenuTransactionId(chatId, transactionId);
            SetOneMenuSaveResponse(transactionId, message, expectedProcessMessageResult);
            
            var processMessageResult = await _processMessage.Execute(chatId, message);

            _conversationState.Verify();
            _oneMenuService.Verify();
            
            Assert.NotNull(processMessageResult);
            Assert.Null(processMessageResult.AvailableMenus);
            Assert.NotEmpty(processMessageResult.Errors);
            Assert.True(processMessageResult.HasErrors);
            Assert.NotNull(processMessageResult.CurrentStep);
            Assert.Equal(Data.step1.Text, processMessageResult.CurrentStep.Text);
        }
        
        [Fact]
        public async Task When_Receives_A_response_For_LastStep_And_Returns_CompletionMsj_And_Resets_ConversationState()
        {
            var chatId = 1L;
            var transactionId = "transactionId000";
            var message = "last step answer";
            var completionMsg = "menu transaction completed";

            var expectedProcessMessageResult = new ProcessMessageResult()
            {
                IsCompleted =  true,
                CompletionMsg = completionMsg
            };

            SetConversationStateReset(chatId);
            SetConversationState(chatId, transactionId);
            SetOneMenuSaveResponse(transactionId, message, expectedProcessMessageResult);

            var processMessageResult = await _processMessage.Execute(chatId, message);

            _conversationState.Verify();
            _oneMenuService.Verify();
            
            Assert.NotNull(processMessageResult);
            Assert.Null(processMessageResult.AvailableMenus);
            Assert.Null(processMessageResult.CurrentStep);
            Assert.True(processMessageResult.IsCompleted);
            Assert.Equal(completionMsg, processMessageResult.CompletionMsg);
        }
        
        private void SetOneMenu_GetAllMenus()
        {
            _oneMenuService
                .Setup(om => om.GetAllMenus())
                .ReturnsAsync(Data.AvailableMenus)
                .Verifiable();
        }

        private void SetConversationState(long chatId, string transactionId)
        {
            _conversationState
                .Setup(cs => cs.GetCurrentMenuTransactionId(chatId))
                .Returns(transactionId)
                .Verifiable();
        }
        
        private void SetConversationStateCurrentTransactionId(long chatId, string newTransactionId)
        {
            _conversationState
                .Setup(cs => cs.SetCurrentMenuTransactionId(chatId, newTransactionId))
                .Verifiable();
        }

        private void SetConversationStateReset(long chatId)
        {
            _conversationState
                .Setup(cs => cs.Reset(chatId))
                .Verifiable();
        }

        private void SetGetCurrentStep(string newTransactionId, Step step)
        {
            _oneMenuService
                .Setup(om => om.GetCurrentStep(newTransactionId))
                .ReturnsAsync(step)
                .Verifiable();
        }

        private void SetInitMenuTransaction(string menuLabel, string newTransactionId)
        {
            _oneMenuService
                .Setup(om => om.InitMenuTransaction(menuLabel))
                .ReturnsAsync(newTransactionId)
                .Verifiable();
        }

        private void SetOneMenuGetCurrentMenuTransactionId(long chatId, string transactionId)
        {
            _conversationState
                .Setup(cs => cs.GetCurrentMenuTransactionId(chatId))
                .Returns(transactionId)
                .Verifiable();
        }

        private void SetOneMenuSaveResponse(string transactionId, string message, ProcessMessageResult expectedProcessMessageResult)
        {
            _oneMenuService
                .Setup(om => om.SaveResponse(transactionId, message))
                .ReturnsAsync(expectedProcessMessageResult)
                .Verifiable();
        }
    }

    public class Data
    {
        public static Menu Menu1 = new()
        {
            Label = "menutest1",
            Text = "this is menu 1",
            Title = "title for menu 1"
        };
        
        public static Menu Menu2 = new()
        {
            Label = "menutest2",
            Text = "this is menu 2",
            Title = "title for menu 2"
        };
        
        public static Menu Menu3 = new()
        {
            Label = "menutest3",
            Text = "this is menu 3",
            Title = "title for menu 3"
        };

        public static List<Menu> AvailableMenus = new() {Menu1, Menu2, Menu3};

        public static Step step1 => new ()
        {
            Text = "Step 1"
        };
        
        public static Step step2 => new ()
        {
            Text = "Step 2"
        };
    }
}