/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Runtime.InteropServices;
using GameEvent;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using OutputEventPauseMessage = Playdek.TwilightStruggle.V1.OutputEventPause;
using OutputEventAnimationCardMessage = Playdek.TwilightStruggle.V1.OutputEventAnimationCard;
using OutputEventAnimationAddInfluenceMessage = Playdek.TwilightStruggle.V1.OutputEventAnimationAddInfluence;
using OutputEventAnimationRemoveInfluenceMessage = Playdek.TwilightStruggle.V1.OutputEventAnimationRemoveInfluence;
using OutputEventAnimationTargetCountryMessage = Playdek.TwilightStruggle.V1.OutputEventAnimationTargetCountry;
using OutputEventAnimationVictoryPointsMessage = Playdek.TwilightStruggle.V1.OutputEventAnimationVictoryPoints;
using CountryDefinitionMessage = Playdek.TwilightStruggle.V1.CountryDefinition;
using CountryInfluenceMessage = Playdek.TwilightStruggle.V1.CountryInfluence;
using CardLocationMessage = Playdek.TwilightStruggle.V1.CardLocation;
using CardInPlayStatusMessage = Playdek.TwilightStruggle.V1.CardInPlayStatus;
using ActionRoundMessage = Playdek.TwilightStruggle.V1.ActionRound;
using PhasingPlayerMessage = Playdek.TwilightStruggle.V1.PhasingPlayer;
using TurnNumberMessage = Playdek.TwilightStruggle.V1.TurnNumber;
using VictoryPointTrackMessage = Playdek.TwilightStruggle.V1.VictoryPointTrack;
using DefconLevelMessage = Playdek.TwilightStruggle.V1.DefconLevel;
using MilitaryOpsMessage = Playdek.TwilightStruggle.V1.MilitaryOps;
using SpaceRaceTrackMessage = Playdek.TwilightStruggle.V1.SpaceRaceTrack;
using CardsSpacedMessage = Playdek.TwilightStruggle.V1.CardsSpaced;
using ChinaCardMessage = Playdek.TwilightStruggle.V1.ChinaCard;
using GameOverMessage = Playdek.TwilightStruggle.V1.GameOver;
using AssignSidesMessage = Playdek.TwilightStruggle.V1.AssignSides;
using RealignmentMessage = Playdek.TwilightStruggle.V1.Realignment;
using PushResolveCardMessage = Playdek.TwilightStruggle.V1.PushResolveCard;
using PopResolveCardMessage = Playdek.TwilightStruggle.V1.PopResolveCard;
using PushRevealCardMessage = Playdek.TwilightStruggle.V1.PushRevealCard;
using PopRevealCardMessage = Playdek.TwilightStruggle.V1.PopRevealCard;
using SetRevealCardPlayerMessage = Playdek.TwilightStruggle.V1.SetRevealCardPlayer;
using SetHeadlineCardRevealedMessage = Playdek.TwilightStruggle.V1.SetHeadlineCardRevealed;
using LoadProgressMessage = Playdek.TwilightStruggle.V1.LoadProgress;
using CommitPlayerDecisionMessage = Playdek.TwilightStruggle.V1.CommitPlayerDecision;
using CoupRollMessage = Playdek.TwilightStruggle.V1.CoupRoll;
using WarRollMessage = Playdek.TwilightStruggle.V1.WarRoll;
using SpaceRaceRollMessage = Playdek.TwilightStruggle.V1.SpaceRaceRoll;
using TrapRollMessage = Playdek.TwilightStruggle.V1.TrapRoll;
using ScoringCardPlayedMessage = Playdek.TwilightStruggle.V1.ScoringCardPlayed;
using FinalScoringMessage = Playdek.TwilightStruggle.V1.FinalScoring;
using EffectRollMessage = Playdek.TwilightStruggle.V1.EffectRoll;
using EndTurnMessage = Playdek.TwilightStruggle.V1.EndTurn;
using HeadlineAnnounceMessage = Playdek.TwilightStruggle.V1.HeadlineAnnounce;
using ReshuffleMessage = Playdek.TwilightStruggle.V1.Reshuffle;
using PauseForRevealedCardsMessage = Playdek.TwilightStruggle.V1.PauseForRevealedCards;
using TutorialAISelectedOptionMessage = Playdek.TwilightStruggle.V1.TutorialAISelectedOption;
using BiddingResultsMessage = Playdek.TwilightStruggle.V1.BiddingResults;
using TurnZeroMessage = Playdek.TwilightStruggle.V1.TurnZero;
using TurnZeroCrisisCardMessage = Playdek.TwilightStruggle.V1.TurnZeroCrisisCard;
using SetStatecraftCardRevealedMessage = Playdek.TwilightStruggle.V1.SetStatecraftCardRevealed;
using CrisisCardRollMessage = Playdek.TwilightStruggle.V1.CrisisCardRoll;

namespace TSEspionage
{
    public class GameEventHandler
    {
        public void HandleEvent(ref IntPtr eventBuffer)
        {
            var eventType = (EventType)Marshal.ReadIntPtr(eventBuffer).ToInt32();
            var eventPointer = eventBuffer + Marshal.SizeOf(typeof(int));

            // var _parser = new global::Google.Protobuf.MessageParser<Playdek.TwilightStruggle.V1.GameEvent>(() => new Playdek.TwilightStruggle.V1.GameEvent());

            var message = new Playdek.TwilightStruggle.V1.GameEvent();
            message.Timestamp = Timestamp.FromDateTime(DateTime.UtcNow);

            switch (eventType)
            {
                case EventType.OutputEventPause:
                    var outputEventPause = Marshal.PtrToStructure<OutputEventPause>(eventPointer);
                    message = ToMessage(outputEventPause);
                    break;
                case EventType.OutputEventAnimationCard:
                    var outputEventAnimationCard = Marshal.PtrToStructure<OutputEventAnimationCard>(eventPointer);
                    message = ToMessage(outputEventAnimationCard);
                    break;
                case EventType.OutputEventAnimationAddInfluence:
                    var outputEventAnimationAddInfluence =
                        Marshal.PtrToStructure<OutputEventAnimationAddInfluence>(eventPointer);
                    message = ToMessage(outputEventAnimationAddInfluence);
                    break;
                case EventType.OutputEventAnimationRemoveInfluence:
                    var outputEventAnimationRemoveInfluence =
                        Marshal.PtrToStructure<OutputEventAnimationRemoveInfluence>(eventPointer);
                    message = ToMessage(outputEventAnimationRemoveInfluence);
                    break;
                case EventType.OutputEventAnimationTargetCountry:
                    var outputEventAnimationTargetCountry =
                        Marshal.PtrToStructure<OutputEventAnimationTargetCountry>(eventPointer);
                    message.OutputEventAnimationTargetCountry = ToMessage(outputEventAnimationTargetCountry);
                    break;
                case EventType.OutputEventAnimationVictoryPoints:
                    var outputEventAnimationVictoryPoints =
                        Marshal.PtrToStructure<OutputEventAnimationVictoryPoints>(eventPointer);
                    message.OutputEventAnimationVictoryPoints = ToMessage(outputEventAnimationVictoryPoints);
                    break;
                case EventType.CountryDefinition:
                    var countryDefinition = Marshal.PtrToStructure<GameEvent.CountryDefinition>(eventPointer);
                    message.CountryDefinition = ToMessage(countryDefinition);
                    break;
                case EventType.CountryInfluence:
                    var countryInfluence = Marshal.PtrToStructure<CountryInfluence>(eventPointer);
                    message.CountryInfluence = ToMessage(countryInfluence);
                    break;
                case EventType.CardLocation:
                    var cardLocation = Marshal.PtrToStructure<CardLocation>(eventPointer);
                    message.CardLocation = ToMessage(cardLocation);
                    break;
                case EventType.CardInPlayStatus:
                    var cardInPlayStatus = Marshal.PtrToStructure<CardInPlayStatus>(eventPointer);
                    message.CardInPlayStatus = ToMessage(cardInPlayStatus);
                    break;
                case EventType.ActionRound:
                    var actionRound = Marshal.PtrToStructure<ActionRound>(eventPointer);
                    message.ActionRound = ToMessage(actionRound);
                    break;
                case EventType.PhasingPlayer:
                    var phasingPlayer = Marshal.PtrToStructure<PhasingPlayer>(eventPointer);
                    message.PhasingPlayer = ToMessage(phasingPlayer);
                    break;
                case EventType.TurnNumber:
                    var turnNumber = Marshal.PtrToStructure<TurnNumber>(eventPointer);
                    message.TurnNumber = ToMessage(turnNumber);
                    break;
                case EventType.VPTrack:
                    var vpTrack = Marshal.PtrToStructure<VPTrack>(eventPointer);
                    message.VictoryPointTrack = ToMessage(vpTrack);
                    break;
                case EventType.DefconLevel:
                    var defconLevel = Marshal.PtrToStructure<DefconLevel>(eventPointer);
                    message.DefconLevel = ToMessage(defconLevel);
                    break;
                case EventType.MilitaryOps:
                    var militaryOps = Marshal.PtrToStructure<MilitaryOps>(eventPointer);
                    message.MilitaryOps = ToMessage(militaryOps);
                    break;
                case EventType.SpaceRaceTrack:
                    var spaceRaceTrack = Marshal.PtrToStructure<SpaceRaceTrack>(eventPointer);
                    message.SpaceRaceTrack = ToMessage(spaceRaceTrack);
                    break;
                case EventType.CardsSpaced:
                    var cardsSpaced = Marshal.PtrToStructure<CardsSpaced>(eventPointer);
                    message.CardsSpaced = ToMessage(cardsSpaced);
                    break;
                case EventType.ChinaCard:
                    var chinaCard = Marshal.PtrToStructure<ChinaCard>(eventPointer);
                    message.ChinaCard = ToMessage(chinaCard);
                    break;
                case EventType.GameOver:
                    var gameOver = Marshal.PtrToStructure<GameOver>(eventPointer);
                    message.GameOver = ToMessage(gameOver);
                    break;
                case EventType.AssignSides:
                    var assignSides = Marshal.PtrToStructure<AssignSides>(eventPointer);
                    message.AssignSides = ToMessage(assignSides);
                    break;
                case EventType.Realignment:
                    var realignment = Marshal.PtrToStructure<Realignment>(eventPointer);
                    message.Realignment = ToMessage(realignment);
                    break;
                case EventType.PushResolveCard:
                    var pushResolveCard = Marshal.PtrToStructure<PushResolveCard>(eventPointer);
                    message.PushResolveCard = ToMessage(pushResolveCard);
                    break;
                case EventType.PopResolveCard:
                    var popResolveCard = Marshal.PtrToStructure<PopResolveCard>(eventPointer);
                    message.PopResolveCard = ToMessage(popResolveCard);
                    break;
                case EventType.PushRevealCard:
                    var pushRevealCard = Marshal.PtrToStructure<PushRevealCard>(eventPointer);
                    message.PushRevealCard = ToMessage(pushRevealCard);
                    break;
                case EventType.PopRevealCard:
                    var popRevealCard = Marshal.PtrToStructure<PopRevealCard>(eventPointer);
                    message.PopRevealCard = ToMessage(popRevealCard);
                    break;
                case EventType.SetRevealCardPlayer:
                    var setRevealCardPlayer = Marshal.PtrToStructure<SetRevealCardPlayer>(eventPointer);
                    message.SetRevealCardPlayer = ToMessage(setRevealCardPlayer);
                    break;
                case EventType.SetHeadlineCardRevealed:
                    var setHeadlineCardRevealed = Marshal.PtrToStructure<SetHeadlineCardRevealed>(eventPointer);
                    message.SetHeadlineCardRevealed = ToMessage(setHeadlineCardRevealed);
                    break;
                case EventType.LoadProgress:
                    var loadProgress = Marshal.PtrToStructure<LoadProgress>(eventPointer);
                    message.LoadProgress = ToMessage(loadProgress);
                    break;
                case EventType.CommitPlayerDecision:
                    var commitPlayerDecision = Marshal.PtrToStructure<CommitPlayerDecision>(eventPointer);
                    message.CommitPlayerDecision = ToMessage(commitPlayerDecision);
                    break;
                case EventType.CoupRoll:
                    var coupRoll = Marshal.PtrToStructure<CoupRoll>(eventPointer);
                    message.CoupRoll = ToMessage(coupRoll);
                    break;
                case EventType.WarRoll:
                    var warRoll = Marshal.PtrToStructure<WarRoll>(eventPointer);
                    message.WarRoll = ToMessage(warRoll);
                    break;
                case EventType.SpaceRaceRoll:
                    var spaceRaceRoll = Marshal.PtrToStructure<SpaceRaceRoll>(eventPointer);
                    message.SpaceRaceRoll = ToMessage(spaceRaceRoll);
                    break;
                case EventType.TrapRoll:
                    var trapRoll = Marshal.PtrToStructure<TrapRoll>(eventPointer);
                    message.TrapRoll = ToMessage(trapRoll);
                    break;
                case EventType.ScoringCardPlayed:
                    var scoringCardPlayed = Marshal.PtrToStructure<ScoringCardPlayed>(eventPointer);
                    message.ScoringCardPlayed = ToMessage(scoringCardPlayed);
                    break;
                case EventType.FinalScoring:
                    var finalScoring = Marshal.PtrToStructure<FinalScoring>(eventPointer);
                    message.FinalScoring = ToMessage(finalScoring);
                    break;
                case EventType.EffectRoll:
                    var effectRoll = Marshal.PtrToStructure<EffectRoll>(eventPointer);
                    message.EffectRoll = ToMessage(effectRoll);
                    break;
                case EventType.EndTurn:
                    var endTurn = Marshal.PtrToStructure<EndTurn>(eventPointer);
                    message.EndTurn = ToMessage(endTurn);
                    break;
                case EventType.HeadlineAnnounce:
                    var headlineAnnounce = Marshal.PtrToStructure<HeadlineAnnounce>(eventPointer);
                    message.HeadlineAnnounce = ToMessage(headlineAnnounce);
                    break;
                case EventType.Reshuffle:
                    var reshuffle = Marshal.PtrToStructure<ShuffledDiscardIntoDeck>(eventPointer);
                    message.Reshuffle = ToMessage(reshuffle);
                    break;
                case EventType.PauseForRevealedCards:
                    var pauseForRevealedCards = Marshal.PtrToStructure<PauseForRevealedCards>(eventPointer);
                    message.PauseForRevealedCards = ToMessage(pauseForRevealedCards);
                    break;
                case EventType.TutorialAISelectedOption:
                    var tutorialAISelectedOption = Marshal.PtrToStructure<TutorialAISelectedOption>(eventPointer);
                    message.TutorialAiSelectedOption = ToMessage(tutorialAISelectedOption);
                    break;
                case EventType.BiddingResults:
                    var biddingResult = Marshal.PtrToStructure<BiddingResult>(eventPointer);
                    message.BiddingResults = ToMessage(biddingResult);
                    break;
                case EventType.TurnZero:
                    var turnZero = Marshal.PtrToStructure<TurnZero>(eventPointer);
                    message.TurnZero = ToMessage(turnZero);
                    break;
                case EventType.TurnZeroCrisisCard:
                    var turnZeroCrisisCard = Marshal.PtrToStructure<TurnZeroCrisisCard>(eventPointer);
                    message.TurnZeroCrisisCard = ToMessage(turnZeroCrisisCard);
                    break;
                case EventType.SetStatecraftCardRevealed:
                    var setStatecraftCardRevealed = Marshal.PtrToStructure<SetStatecraftCardRevealed>(eventPointer);
                    message.SetStatecraftCardRevealed = ToMessage(setStatecraftCardRevealed);
                    break;
                case EventType.CrisisCardRoll:
                    var crisisCardRoll = Marshal.PtrToStructure<CrisisCardRoll>(eventPointer);
                    message.CrisisCardRoll = ToMessage(crisisCardRoll);
                    break;
                default:
                    return;
            }

            var gameId = TwilightLib.GetCurrentGameID();
            try
            {
                var eventWriter = GameReplay.GetGameReplayWriter(gameId);
                eventWriter.WriteGameEvent(message);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed writing game event: {e}");
            }
        }
        
        private static Playdek.TwilightStruggle.V1.GameEvent ToMessage(OutputEventPause outputEventPause)
        {
            var message = new Playdek.TwilightStruggle.V1.GameEvent();
            message.Timestamp = Timestamp.FromDateTime(DateTime.UtcNow);
            message.OutputEventPause = new OutputEventPauseMessage
            {
                AnimationData = outputEventPause.animation_data,
                PauseType = outputEventPause.pause_type,
                ExcludePlayerIndex = outputEventPause.exclude_player_index
            };

            return message;
        }

        private static Playdek.TwilightStruggle.V1.GameEvent ToMessage(
            OutputEventAnimationCard outputEventAnimationCard)
        {
            var message = new Playdek.TwilightStruggle.V1.GameEvent();
            message.Timestamp = Timestamp.FromDateTime(DateTime.UtcNow);
            message.OutputEventAnimationCard = new OutputEventAnimationCardMessage
            {
                CardInstanceId = outputEventAnimationCard.card_instance_id,
                AnimationDestinationLocation = outputEventAnimationCard.animation_destination_location,
                AnimationDestinationInstanceId = outputEventAnimationCard.animation_destination_instance_id,
                AnimationEventHint = outputEventAnimationCard.animation_event_hint,
                AnimationSourceInstanceId = outputEventAnimationCard.animation_source_instance_id,
                AnimationSourceLocation = outputEventAnimationCard.animation_source_location
            };

            return message;
        }

        private static Playdek.TwilightStruggle.V1.GameEvent ToMessage(
            OutputEventAnimationAddInfluence outputEventAnimationAddInfluence)
        {
            var message = new Playdek.TwilightStruggle.V1.GameEvent();
            message.Timestamp = Timestamp.FromDateTime(DateTime.UtcNow);
            message.OutputEventAnimationAddInfluence = new OutputEventAnimationAddInfluenceMessage
            {
                SourcePlayerIndex = outputEventAnimationAddInfluence.source_player_index,
                AnimationEventHint = outputEventAnimationAddInfluence.animation_event_hint,
                CountryInstanceId = outputEventAnimationAddInfluence.country_instance_id,
                InfluenceCount = outputEventAnimationAddInfluence.influence_count,
                SourceCardInstanceId = outputEventAnimationAddInfluence.source_player_index
            };

            return message;
        }

        private static Playdek.TwilightStruggle.V1.GameEvent ToMessage(OutputEventAnimationRemoveInfluence ev)
        {
            var message = new Playdek.TwilightStruggle.V1.GameEvent();
            message.Timestamp = Timestamp.FromDateTime(DateTime.UtcNow);
            message.OutputEventAnimationRemoveInfluence = new OutputEventAnimationRemoveInfluenceMessage
            {
                SourcePlayerIndex = ev.source_player_index,
                AnimationEventHint = ev.animation_event_hint,
                CountryInstanceId = ev.country_instance_id,
                InfluenceCount = ev.influence_count,
                SourceCardInstanceId = ev.source_card_instance_id
            };

            return message;
        }

        private static OutputEventAnimationTargetCountryMessage ToMessage(OutputEventAnimationTargetCountry ev)
        {
            return new OutputEventAnimationTargetCountryMessage
            {
                SourcePlayerIndex = ev.source_player_index,
                AnimationEventHint = ev.animation_event_hint,
                CountryInstanceId = ev.country_instance_id,
                SourceCardInstanceId = ev.source_card_instance_id,
                TargetType = ev.target_type
            };
        }

        private static OutputEventAnimationVictoryPointsMessage ToMessage(OutputEventAnimationVictoryPoints ev)
        {
            return new OutputEventAnimationVictoryPointsMessage
            {
                SourceCardInstanceId = ev.source_card_instance_id,
                AnimationEventHint = ev.animation_event_hint,
                VictoryPointCount = ev.victory_point_count
            };
        }

        private static CountryDefinitionMessage ToMessage(GameEvent.CountryDefinition ev)
        {
            return new CountryDefinitionMessage
            {
                Id = ev.id,
                Battleground = ev.battleground,
                Stability = ev.stability
            };
        }

        private static CountryInfluenceMessage ToMessage(CountryInfluence ev)
        {
            return new CountryInfluenceMessage
            {
                Id = ev.id,
                UsaInfluence = ev.us_influence,
                UssrInfluence = ev.ussr_influence
            };
        }

        private static CardLocationMessage ToMessage(CardLocation ev)
        {
            return new CardLocationMessage
            {
                Id = ev.id,
                DoNotAnimate = ev.bDoNotAnimate,
                Location = ev.location
            };
        }

        private static CardInPlayStatusMessage ToMessage(CardInPlayStatus ev)
        {
            return new CardInPlayStatusMessage
            {
                CardInPlayInstanceId = ev.cardinplay_instance_id,
                DurationType = ev.duration_type,
                InPlay = ev.inplay,
                OwnerIndex = ev.owner_index,
                SourceCardInstanceId = ev.sourcecard_instance_id
            };
        }

        private static ActionRoundMessage ToMessage(ActionRound ev)
        {
            return new ActionRoundMessage
            {
                IsSimulating = ev.isSimulating,
                ActionRound_ = ev.action_round,
                AffectedByKremlinFlu = ev.affected_by_kremlin_flu,
                AffectedByMissileEnvy = ev.affected_by_missile_envy,
                EndOfTurn = ev.end_of_turn,
                PhasingPlayerSuperpower = ev.phasing_player_superpower,
                PlayerId = ev.player_ID,
                ScoringCardCount = ev.scoring_card_count
            };
        }

        private static PhasingPlayerMessage ToMessage(PhasingPlayer ev)
        {
            return new PhasingPlayerMessage
            {
                PhasingPlayer_ = ev.phasing_player
            };
        }

        private static TurnNumberMessage ToMessage(TurnNumber ev)
        {
            return new TurnNumberMessage
            {
                IsSimulating = ev.isSimulating,
                HasExtraRound = ev.has_extra_round,
                TurnNumber_ = ev.turn_number,
                UssrHand = ev.ussr_hand,
                UsaHand = ev.usa_hand
            };
        }

        private static VictoryPointTrackMessage ToMessage(VPTrack ev)
        {
            return new VictoryPointTrackMessage
            {
                VictoryPointTrack_ = ev.vp_track
            };
        }

        private static DefconLevelMessage ToMessage(DefconLevel ev)
        {
            return new DefconLevelMessage
            {
                IsSimulating = ev.isSimulating,
                DefconLevel_ = ev.defcon_level
            };
        }

        private static MilitaryOpsMessage ToMessage(MilitaryOps ev)
        {
            return new MilitaryOpsMessage
            {
                UssrMilops = ev.ussr_milops,
                UsaMilops = ev.us_milops
            };
        }

        private static SpaceRaceTrackMessage ToMessage(SpaceRaceTrack ev)
        {
            return new SpaceRaceTrackMessage
            {
                UssrSpace = ev.ussr_space,
                UsaSpace = ev.us_space
            };
        }

        private static CardsSpacedMessage ToMessage(CardsSpaced ev)
        {
            return new CardsSpacedMessage
            {
                UssrCardsSpaced = ev.ussr_cards_spaced,
                UsaCardsSpaced = ev.us_cards_spaced
            };
        }

        private static ChinaCardMessage ToMessage(ChinaCard ev)
        {
            return new ChinaCardMessage
            {
                InstanceId = ev.instanceID,
                FaceUp = ev.faceup,
                Player = ev.player
            };
        }

        private static GameOverMessage ToMessage(GameOver ev)
        {
            return new GameOverMessage
            {
                Winner = ev.winner,
                WinType = ev.win_type
            };
        }

        private static AssignSidesMessage ToMessage(AssignSides ev)
        {
            return new AssignSidesMessage
            {
                UssrPlayerId = ev.USSRPlayerID
            };
        }

        private static RealignmentMessage ToMessage(Realignment ev)
        {
            return new RealignmentMessage
            {
                RealignPlayerIndex = ev.realign_player_index,
                Country = ev.country,
                UsaRollResult = ev.US_roll_result,
                UssrRollResult = ev.USSR_roll_result
            };
        }

        private static PushResolveCardMessage ToMessage(PushResolveCard ev)
        {
            return new PushResolveCardMessage
            {
                Card = ev.card
            };
        }

        private static PopResolveCardMessage ToMessage(PopResolveCard ev)
        {
            return new PopResolveCardMessage
            {
                Card = ev.card
            };
        }

        private static PushRevealCardMessage ToMessage(PushRevealCard ev)
        {
            return new PushRevealCardMessage
            {
                Card = ev.card
            };
        }

        private static PopRevealCardMessage ToMessage(PopRevealCard ev)
        {
            return new PopRevealCardMessage
            {
                Card = ev.card
            };
        }

        private static SetRevealCardPlayerMessage ToMessage(SetRevealCardPlayer ev)
        {
            return new SetRevealCardPlayerMessage
            {
                PlayerIndex = ev.player_index
            };
        }

        private static SetHeadlineCardRevealedMessage ToMessage(SetHeadlineCardRevealed ev)
        {
            return new SetHeadlineCardRevealedMessage
            {
                PlayerIndex = ev.player_index,
                Revealed = ev.revealed
            };
        }

        private static LoadProgressMessage ToMessage(LoadProgress ev)
        {
            return new LoadProgressMessage
            {
                Progress = ev.progress
            };
        }

        private static CommitPlayerDecisionMessage ToMessage(CommitPlayerDecision ev)
        {
            return new CommitPlayerDecisionMessage
            {
                MoveCount = ev.moveCount,
                WinnerPlayerIndex = ev.winnerPlayerIndex
            };
        }

        private static CoupRollMessage ToMessage(CoupRoll ev)
        {
            return new CoupRollMessage
            {
                PlayerIndex = ev.coup_player_index,
                CountryId = ev.country_id,
                Roll = ev.roll
            };
        }

        private static WarRollMessage ToMessage(WarRoll ev)
        {
            return new WarRollMessage
            {
                PlayerIndex = ev.player_index,
                CountryId = ev.country_id,
                Roll = ev.roll
            };
        }

        private static SpaceRaceRollMessage ToMessage(SpaceRaceRoll ev)
        {
            return new SpaceRaceRollMessage
            {
                IsSimulating = ev.isSimulating,
                Card = ev.card,
                Roll = ev.roll,
                SpaceRaceAdvanceGainBonus = ev.space_race_advance_gain_bonus,
                SpaceRaceAdvanceRemoveBonus = ev.space_race_advance_remove_bonus,
                SpaceRaceAdvanceVictoryPoints = ev.space_race_advance_victory_points,
                SpaceRaceCurrentLevel = ev.space_race_current_level,
                SpaceRaceNextLevel = ev.space_race_next_level,
                SpaceRaceOpponentLevel = ev.space_race_opponent_level,
                SpaceRacePlayerIndex = ev.space_race_player_index
            };
        }

        private static TrapRollMessage ToMessage(TrapRoll ev)
        {
            return new TrapRollMessage
            {
                PlayerId = ev.player_id,
                Roll = ev.roll,
                TrapDiscardCardId = ev.trap_discard_card_id,
                TrapEscapeRollTarget = ev.trap_escape_roll_target,
                TrapRequiredOperationsPoints = ev.trap_required_operations_points,
                TrapSourceCardId = ev.trap_source_card_id
            };
        }

        private static ScoringCardPlayedMessage ToMessage(ScoringCardPlayed ev)
        {
            return new ScoringCardPlayedMessage
            {
                CardId = ev.card_id,
                PlayerId = ev.player_id
            };
        }

        private static FinalScoringMessage ToMessage(FinalScoring ev)
        {
            return new FinalScoringMessage
            {
                VictoryPointsUsa = ev.victory_points_usa,
                VictoryPointsUssr = ev.victory_points_ussr,
                EuropeanControlWinner = ev.european_control_winner
            };
        }

        private static EffectRollMessage ToMessage(EffectRoll ev)
        {
            return new EffectRollMessage
            {
                CardId = ev.card_id,
                UsaModify = ev.usa_modify,
                UsaRoll = ev.usa_roll,
                UssrModify = ev.ussr_modify,
                UssrRoll = ev.ussr_roll
            };
        }

        private static EndTurnMessage ToMessage(EndTurn ev)
        {
            return new EndTurnMessage
            {
                Defcon = ev.defcon,
                SpaceRace = ev.space_race,
                UsaOps = ev.usa_ops,
                UssrOps = ev.ussr_ops
            };
        }

        private static HeadlineAnnounceMessage ToMessage(HeadlineAnnounce ev)
        {
            return new HeadlineAnnounceMessage
            {
                SpaceRace = ev.space_race
            };
        }

        private static ReshuffleMessage ToMessage(ShuffledDiscardIntoDeck ev)
        {
            return new ReshuffleMessage
            {
                Zero = ev.zero
            };
        }

        private static PauseForRevealedCardsMessage ToMessage(PauseForRevealedCards ev)
        {
            return new PauseForRevealedCardsMessage
            {
                PlayerIndex = ev.player_index
            };
        }

        private static TutorialAISelectedOptionMessage ToMessage(TutorialAISelectedOption ev)
        {
            return new TutorialAISelectedOptionMessage
            {
                SelectionId = ev.selection_id,
                SelectionHint = ev.selection_hint
            };
        }

        private static BiddingResultsMessage ToMessage(BiddingResult ev)
        {
            return new BiddingResultsMessage
            {
                Player1Id = ev.player1_id,
                Player1BidSide = ev.player1_bidSide,
                Player1Bid = ev.player1_bid,
                Player2Id = ev.player2_id,
                Player2BidSide = ev.player1_bidSide,
                Player2Bid = ev.player2_bid
            };
        }

        private static TurnZeroMessage ToMessage(TurnZero ev)
        {
            return new TurnZeroMessage
            {
                Begin = ev.begin
            };
        }

        private static TurnZeroCrisisCardMessage ToMessage(TurnZeroCrisisCard ev)
        {
            return new TurnZeroCrisisCardMessage
            {
                CrisisCardInstanceId = ev.crisis_card_instance_id
            };
        }

        private static SetStatecraftCardRevealedMessage ToMessage(SetStatecraftCardRevealed ev)
        {
            return new SetStatecraftCardRevealedMessage
            {
                PlayerIndex = ev.player_index,
                Revealed = ev.revealed
            };
        }

        private static CrisisCardRollMessage ToMessage(CrisisCardRoll ev)
        {
            return new CrisisCardRollMessage
            {
                CrisisCardInstanceId = ev.crisis_card_instance_id,
                CrisisResultIndex = ev.crisis_result_index,
                DieRoll = ev.die_roll,
                FinalResult = ev.final_result,
                ModifierUsa = ev.modifier_usa,
                ModifierUssr = ev.modifier_ussr
            };
        }
    }
}
