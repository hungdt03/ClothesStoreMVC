
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Models;
using WebBanQuanAo.Services.Interfaces;
using WebBanQuanAo.SignalR.Payload;

namespace WebBanQuanAo.SignalR.Hubs
{
    public class MessageHub : Hub
    {
        private readonly IGroupService groupService;
        private readonly IMessageService messageService;
        private readonly IConnectionService connectionService;
        private readonly PresenceTracker presenceTracker;
        private readonly UserManager<User> _userManager;

        public MessageHub(IGroupService groupService, UserManager<User> userManager, IMessageService messageService, IConnectionService connectionService, PresenceTracker presenceTracker)
        {
            this.groupService = groupService;
            this.messageService = messageService;
            this._userManager = userManager;
            this.connectionService = connectionService;
            this.presenceTracker = presenceTracker;
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User.GetUsername();
            presenceTracker.UserConnected(username, Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public async Task SendMessage(MessageDto messageDto)
        {
            var currentUser = Context.User.GetUsername();

            if(currentUser == messageDto.RecipientUsername)
            {
                throw new Exception("Không thể gửi tin nhắn cho chính mình");
            }

            User senderUser = await _userManager.FindByNameAsync(currentUser);
            User recipientUser = await _userManager.FindByNameAsync(messageDto.RecipientUsername);

            if (recipientUser == null)
                throw new BadHttpRequestException("Recipient user haven't existed");

            var groupName = GetGroupName(currentUser, messageDto.RecipientUsername);
            Group? existedGroup = await groupService.FindGroupByGroupName(groupName);

            if (existedGroup == null)
            {
                Connection connection = new Connection()
                {
                    ConnectionId = Context.ConnectionId,
                    Username = currentUser,
                };

                List<Connection> connections = new List<Connection>();
                connections.Add(connection);
                
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                var recipientConnections = await presenceTracker.GetConnectionsForUser(recipientUser.UserName);
                if (recipientConnections != null && recipientConnections.Any())
                {
                    foreach (var connectionId in recipientConnections)
                    {
                        connections.Add(new Connection
                        {
                            ConnectionId = connectionId,
                            Username = recipientUser.UserName
                        });

                        await Groups.AddToGroupAsync(connectionId, groupName);
                    }
                }

                Group group = new Group()
                {
                    GroupName = groupName,
                    Connections = connections
                };
                await groupService.AddToGroup(group);
            } else
            {
                if (!existedGroup.Connections.Any(conn => conn.ConnectionId.Equals(Context.ConnectionId)))
                {
                    existedGroup.Connections.Add(new Connection
                    {
                        ConnectionId = Context.ConnectionId,
                        Username = currentUser,
                    });

                    await groupService.UpdateGroup(existedGroup);
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

                }
            }

            Message message = new Message()
            {
                SenderId = senderUser.Id,
                RecipientId = recipientUser.Id,
                Content = messageDto.Content,
                SendAt = DateTime.Now,
            };

            Message savedMessage = await messageService.AddMessage(message);

            await Clients.Group(groupName).SendAsync("NewMessage", MapMessageDTO(savedMessage));
       
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User.GetUsername();
            presenceTracker.UserDisconnected(username, Context.ConnectionId);
            connectionService.RemoveConnection(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private object MapMessageDTO(Message message)
        {
            return new
            {
                Id = message.Id,
                SenderUser = message.Sender != null ? new
                {
                    Id = message.Sender.Id,
                    FullName = message.Sender.FullName,
                    UserName = message.Sender.UserName,
                } : null,
                RecipientUser = message.Recipient != null ? new
                {
                    Id = message.Recipient.Id,
                    FullName = message.Recipient.FullName,
                    UserName = message.Recipient.UserName,
                } : null,
                Content = message.Content,
                SentAt = message.SendAt,
            };
        }
        
    }
}
