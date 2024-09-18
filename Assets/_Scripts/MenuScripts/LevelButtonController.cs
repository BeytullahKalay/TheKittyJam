using UnityEngine;


namespace MenuScripts
{
    public class LevelButtonController : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LevelButton[] levelButtonList;

        private int _currentLevelIndex;

        private void Start()
        {
            ResetYPositions();
            _currentLevelIndex = GetLevel();
            InitializeLevelButttons();
            InitializeLevelObject();
        }

        private int GetLevel()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefNames.Level))
            {
                PlayerPrefs.SetInt(PlayerPrefNames.Level, 0);
                return 0;
            }
            else
            {
                return PlayerPrefs.GetInt(PlayerPrefNames.Level);
            }
        }

        private void InitializeLevelButttons()
        {
            for (int i = 0; i < levelButtonList.Length; i++)
            {
                levelButtonList[i].InitializeButton(i);
            }
        }

        private void InitializeLevelObject()
        {
            for (int i = 0; i < levelButtonList.Length; i++)
            {
                if (_currentLevelIndex == i)
                    levelButtonList[i].SetButtonAvailable();
                else if (_currentLevelIndex > i)
                    levelButtonList[i].SetButtonPlayed();
                else if (_currentLevelIndex < i)
                    levelButtonList[i].SetButtonLocked();
            }
        }

        private void ResetYPositions()
        {
            var positionCount = _lineRenderer.positionCount;
            var positions = new Vector3[positionCount];
            _lineRenderer.GetPositions(positions);

            for (int i = 0; i < positionCount; i++)
            {
                positions[i].y = 0f;
            }

            _lineRenderer.SetPositions(positions);
        }

        public Transform GetFirstLevelButtonObjectTransform()
        {
            return levelButtonList[0].transform;
        }

        public Transform GetLastLevelButtonObjectTransform()
        {
            return levelButtonList[levelButtonList.Length - 1].transform;
        }

        public Transform GetLevelButtonObjWorldPos(int index)
        {
            return levelButtonList[index].transform;
        }

    }
}
