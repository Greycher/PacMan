using CMP.Scripts.Character;

namespace CMP.Scripts.AiStates
{
    public class GhostBlackboard
    {
        private readonly GridData _gridData;
        private readonly CharacterNavigator _characterNavigator;
        public GhostBlackboard(GridData gridData, CharacterNavigator characterNavigator)
        {
            _gridData = gridData;
            _characterNavigator = characterNavigator;
        }
        
        public GridData GridData => _gridData;
        public CharacterNavigator CharacterNavigator => _characterNavigator;
    }
}