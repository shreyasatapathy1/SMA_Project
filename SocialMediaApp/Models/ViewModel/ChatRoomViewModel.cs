﻿using System;
using System.Collections.Generic;

namespace SocialMediaApp.Models.ViewModel
{
    public class ChatRoomViewModel
    {
        public string FriendUserId { get; set; }
        public string FriendUserName { get; set; }

        public List<ChatMessageViewModel> Messages { get; set; } = new();
    }

    public class ChatMessageViewModel
    {
        public int Id { get; set; } 

        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSentByCurrentUser { get; set; }

        public string? Reaction { get; set; }
        public Post? SharedPost { get; set; }
    }

}
