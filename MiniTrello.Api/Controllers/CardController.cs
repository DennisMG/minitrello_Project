﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using FizzWare.NBuilder;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers
{
    public class CardController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;

        public CardController(IWriteOnlyRepository writeOnlyRepository, IReadOnlyRepository readOnlyRepository, IMappingEngine mappingEngine)
        {
            _writeOnlyRepository = writeOnlyRepository;
            _readOnlyRepository = readOnlyRepository;
            _mappingEngine = mappingEngine;
        }

        [POST("addcard/{LaneId}/{token}")]
        public CardModel AddNewCard([FromBody] CardModel model, string token, long LaneId)
        {
            var session = NewValidSession(token);
            var Lane = _readOnlyRepository.GetById<Lane>(LaneId);
            if(Lane==null)
                throw new BadRequestException("Lane could not be found");

            var newCard = new Card {Content = model.Content};
            Lane.AddCard(newCard);
            var CardCreated = _writeOnlyRepository.Create(newCard);
            return new CardModel {Content = CardCreated.Content,Id = CardCreated.Id};
        }

        [AcceptVerbs(new[] { "DELETE" })]
        [DELETE("card/{CardId}/{Token}")]
        public CardArchiveModel Archive(long CardId,string Token)
        {
            var session = NewValidSession(Token);
            var card = _readOnlyRepository.GetById<Card>(CardId);
            //VerifyAdministrator(board.Administrator, session.User);
            //var card = _readOnlyRepository.GetById<Card>(model.Id);
            if (card == null)
                throw new BadRequestException("Failed to find card");
            card.IsArchived = true;
            var archivedCard = _writeOnlyRepository.Update(card);
            return _mappingEngine.Map<Card, CardArchiveModel >(archivedCard);
        }

        [AcceptVerbs(new[] {"PUT"})]
        [PUT("card/movecard/{Token}")]
        public HttpResponseMessage MoveCard([FromBody] CardtoMoveModel model, string Token)
        {
            var session = NewValidSession(Token);
            var CardtoMove = _readOnlyRepository.GetById<Card>(model.CardId);
            var LaneFrom = _readOnlyRepository.GetById<Lane>(model.From_LaneId);
            var LaneTo = _readOnlyRepository.GetById<Lane>(model.To_LaneId);
            LaneFrom.RemoveCard(CardtoMove);
            LaneTo.AddCard(CardtoMove);
            var laneUpdated = _writeOnlyRepository.Update(LaneFrom);
            var laneUpdated2 = _writeOnlyRepository.Update(LaneTo);
            return new HttpResponseMessage(HttpStatusCode.OK);


        }

        [GET("getcards/{IdLane}/{Token}")]
        public List<CardModel> GetAllForUser(string Token, long IdLane)
        {
            //var session = NewValidSession(Token);
            //ValidateSession(session);
            var lane = _readOnlyRepository.GetById<Lane>(IdLane);
            //List<CardModel> Lista 
            List<CardModel> cardList=new List<CardModel>();
            /*foreach (var card in lane.Cards)
            {
                //cardList.Add(card);
            }*/
            if (lane != null)
            {
                var mappedCardModelList = _mappingEngine.Map<IEnumerable<Card>, IEnumerable<CardModel>>(lane.Cards);
                cardList = mappedCardModelList.Where(card => !card.IsArchived).ToList();
                return cardList;
            }
            return cardList;
            //return mappedCardModelList;
            var cards = Builder<CardModel>.CreateListOfSize(10).Build().ToList();
            return cards;
        }

        public Sessions NewValidSession(string token)
        {

            var session = _readOnlyRepository.First<Sessions>(session1 => token == session1.Token);
            ValidateSession(session);
            return session;
        }

        public void ValidateSession(Sessions session)
        {
            if (session == null || !session.IsTokenActive())
                throw new BadRequestException("Session has expired. Please login again.");
        }

        public void VerifyAdministrator(Accounts administrator, Accounts user)
        {
            if (administrator != user)
                throw new BadRequestException("You don't have the privileges to do this");
        }
    }

    public class CardtoMoveModel
    {
        public long CardId;
        public long From_LaneId;
        public long To_LaneId;
    }
}
