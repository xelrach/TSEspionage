using HarmonyLib;

namespace TSEspionage
{
    public static class GameWorldPatches
    {
        public static void Init()
        {
        }

        [HarmonyPatch(typeof(GameWorld), nameof(GameWorld.Rebuild))]
        public static class RebuildPatch
        {
            /**
             * Record the board state
             */
            public static void Postfix(GameWorld gameWorld)
            {
                GameBoard board = GameBoard.create();
                board.setTurn(gameWorld.m_Turn);
                board.setActionRound(gameWorld.m_ActionRound);
                board.setEndOfTurn(gameWorld.m_EndOfTurn);
                board.setPhasingPlayer(gameWorld.m_PhasingPlayer);
                board.setVictoryPoints(gameWorld.m_VPTrack);
                board.setDefcon(gameWorld.m_DefconLevel);
                board.setUsaMilOps(gameWorld.m_MilitaryOps[1]);
                board.setUssrMilOps(gameWorld.m_MilitaryOps[0]);
                board.setUsaSpace(gameWorld.m_SpaceRaceTrack[1]);
                board.setUssrSpace(gameWorld.m_SpaceRaceTrack[0]);
                board.setUsaSpacedCards(gameWorld.m_CardsSpaced[1]);
                board.setUssrSpacedCards(gameWorld.m_CardsSpaced[0]);

                for (int cardId = (int)ECard.FIRST; cardId < (int)ECard.COUNT; cardId++)
                {
                    board.setCardLocation(cardId, (int)gameWorld.m_CardLocation[cardId]);
                }

                var chinaCardFaceDown = gameWorld.m_CardObject[(int)ECard.THECHINACARD]
                    .GetComponent<TwilightCard>()
                    .m_FacedownLabel
                    .activeSelf;
                board.setChinaCardFaceUp(!chinaCardFaceDown);
            }
        }
    }
}
