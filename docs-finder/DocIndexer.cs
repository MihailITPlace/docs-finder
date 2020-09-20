using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using docs_finder.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace docs_finder
{
    public class DocIndexer
    {
        private VkApi _api;

        public DocIndexer(VkApi api)
        {
            _api = api;
        }

        public void IndexNewDocs()
        {
            using var ctx = new DocContext();
            var ld = ctx.Dates.First();
            
            if (!_api.IsAuthorized)
            {
                Helpers.ColoredWriteLine("Авторизация не удалась. Невозможно проиндексировать новые документы :(", ConsoleColor.Cyan) ;
                Helpers.ColoredWriteLine($"Дата последнего проверенного сообщения: {ld.Date.ToLocalTime()}.", ConsoleColor.Cyan) ;
                return;
            }

            var (lastDate, newPeers) = GetUpdatedPeerIds(ld.Date);
            ld.Date = lastDate;
            ctx.Dates.Update(ld);
            
            Console.WriteLine($"Найдено {newPeers.Count} обновлённых бесед");

            int sum = 0;
            foreach (var peerId in newPeers)
            {
                var peersDocs = ctx.Docs.Where(d => d.PeerId == peerId);
                var lastDocDate = peersDocs.Any() ? peersDocs.Max(d => d.Date) : DateTime.MinValue;

                var docs = GetNewDocsForPeer(peerId, lastDocDate);
                sum += docs.Count;
                ctx.Docs.AddRange(docs);
            }

            ctx.SaveChanges();
            Console.WriteLine($"Добавлено {sum} новых документов");
        }

        private (DateTime, List<long>) GetUpdatedPeerIds(DateTime lastDate)
        {
            GetConversationsResult conversations = null;
            var result = new List<long>();
            ulong? offset = 0;

            do
            {
                conversations = _api.Messages.GetConversations(new GetConversationsParams
                {
                    Count = 200,
                    Extended = false,
                    Offset = offset
                });

                result.AddRange(
                    conversations.Items
                        .Where(item => item.LastMessage.Date > lastDate)
                        .Select(item => item.Conversation.Peer.Id).ToList()
                    );

                var tmpDate = conversations.Items.Max(i => i.LastMessage.Date);
                lastDate = lastDate.Date > tmpDate.Value ? lastDate : tmpDate.Value;
                
                offset += 200;
                
                Thread.Sleep(500);
            } while (conversations.Items.Count == 200);

            return (lastDate, result);
        }

        private List<Doc> GetNewDocsForPeer(long peerId, DateTime lastDocDate)
        {
            var start = "";
            var next = "";

            var res = new List<Doc>();
            do
            {
                var attachments = _api.Messages.GetHistoryAttachments(new MessagesGetHistoryAttachmentsParams
                {
                    Count = 200,
                    StartFrom = start,
                    MediaType = MediaType.Doc,
                    PeerId = peerId
                }, out next);
                start = next;

                res.AddRange(attachments
                    .Where(a => (a.Attachment.Instance is Document))
                    .Select(a => (Document) a.Attachment.Instance)
                    .Where(d => d.Date.Value > lastDocDate)
                    .Select(d => new Doc
                {
                    Date = d.Date.Value,
                    Title = d.Title,
                    LoweredTitle = d.Title.ToLower(),
                    Uri = d.Uri,
                    PeerId = peerId
                }));
                
                Thread.Sleep(400);
            } while (!string.IsNullOrEmpty(next));

            return res;
        }

    }
    
}