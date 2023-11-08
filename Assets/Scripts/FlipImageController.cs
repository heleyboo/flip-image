using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

namespace Tuong
{
    public class FlipImageController: MonoBehaviour
    {
        [SerializeField] protected Transform template;
        [SerializeField] protected Transform container;
        [SerializeField] protected Text txtScore;
        [SerializeField] protected Text txtTurnCount;
        [SerializeField] protected Text txtCorrectPairCount;
        [SerializeField] protected Transform newGamePanel;
        [SerializeField] protected Text txtNewGame;
        [SerializeField] protected Button btnNewGame;
        [SerializeField] protected Button btnNewGameLevel1;
        [SerializeField] protected Button btnNewGameLevel2;
        [SerializeField] protected Button btnQuit;
        [SerializeField] protected AudioSource audioSource;
        public AudioClip clipOver;
        public AudioClip clipWrong;
        public AudioClip clipCorrect;
        public AudioClip clipWin;
        public float volume = 0.5f;

        private List<int> Board { get; set; }
        
        private List<Transform> Items { get; set; }
        
        private List<int> FlippedIndices { get; set; }
        
        private IDictionary<int, Level> _levels = new Dictionary<int, Level>();

        private Level _selectedLevel;
        
        const int Turns = 20;

        private int _firstSelected = -1;
        private int _secondSelected = -1;
        private bool _canSelect = true;
        private int _turns = Turns;
        private int _correctPairs = 0;
        private int _score = 0;
        
        private void NewGame()
        {
            Debug.Log("New Game");
            FlipAll();
            Board = new List<int>();
            FlippedIndices = new List<int>();

            
            List<int> numbers = Enumerable.Range(0, _selectedLevel.Pairs).ToList();

            Random random = new Random();

            // Add each number twice to the list
            foreach (int number in numbers)
            {
                Board.Add(number);
                Board.Add(number);
            }

            // Shuffle the list to randomize positions
            for (int i = Board.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                int board1 = Board[i];
                int board2 = Board[j];
                (board1, board2) = (board2, board1);
                Board[i] = board1;
                Board[j] = board2;
            }

            _firstSelected = -1;
            _secondSelected = -1;
            _canSelect = true;
            _turns = _selectedLevel.Turns;
            _correctPairs = 0;
            _score = 0;
            newGamePanel?.gameObject.SetActive(false);
        }

        public void Awake()
        {
            Items = new List<Transform>();
            btnNewGameLevel1.onClick.AddListener(() => NewGameAtLevel(1));
            btnNewGameLevel2.onClick.AddListener(() => NewGameAtLevel(2));
            btnQuit.onClick.AddListener(() => Quit());
            InitLevels();
            InitBoard();
            NewGameAtLevel(1);
        }

        private void Quit()
        {
            Application.Quit();
        }
        
        private void NewGameAtLevel(int level)
        {
            _selectedLevel = GetLevel(level);
            container.GetComponent<RectTransform>().sizeDelta = new Vector2(_selectedLevel.SizeX, 800);
            for (int i = 0; i < Items.Count; i++)
            {
                var entryTransform = Items[i];
                if (i>=_selectedLevel.Pairs*2)
                {
                    entryTransform.gameObject.SetActive(false);
                }
                else
                {
                    entryTransform.gameObject.SetActive(true);
                }
            }
            NewGame();
        }

        private void InitLevels()
        {
            _levels.Add(1, new Level() { Pairs = 6, Turns = 12, SizeX = -300 });
            _levels.Add(2, new Level() { Pairs = 10, Turns = 20, SizeX = -100 });
        }

        private Level GetLevel(int level)
        {
            return _levels[level];
        }
            

        private void ShowPanelNewGame(bool isWin = true)
        {
            newGamePanel.gameObject.SetActive(true);
            if (isWin)
            {
                txtNewGame.text = string.Format("You win! Your score is: {0}", _score);
            }
            else
            {
                txtNewGame.text = "You lose!";
            }
            btnNewGame.onClick.AddListener(() => NewGame());
        }

        public void InitBoard()
        {
            for (int i = 0; i < 20; i++)
            {
                Transform entryTransform = Instantiate(template, container);
                entryTransform.gameObject.SetActive(true);
                Items.Add(entryTransform);
                
                int index = i;

                // entryTransform.GetComponent<Button>().onClick.AddListener(() => HandleClick(index));
                entryTransform.GetComponent<Button>().onClick.AddListener(() => clickHandler.OnButtonClick(index));
                
                
                // Assuming you have a reference to the sprite you want to set
                var imageObj = entryTransform.Find("Image").GetComponent<Image>();
                // imageObj.sprite = Resources.Load<Sprite>(String.Format("Sprites/{0}", Board[i]));
                imageObj.enabled = true;
            }
        }

        private void Update()
        {
            txtScore.text = string.Format("Score: {0}", _score);
            txtTurnCount.text = string.Format("Remaining Turns: {0}", _turns);
            txtCorrectPairCount.text = string.Format("Correct pairs: {0}", _correctPairs);
        }

        private void CheckGame()
        {
            if (_correctPairs == _selectedLevel.Pairs && _turns >= 0)
            {
                audioSource.PlayOneShot(clipWin, volume);
                ShowPanelNewGame();
            }
            if (_turns == 0 && _correctPairs < _selectedLevel.Pairs)
            {
                audioSource.PlayOneShot(clipOver, volume);
                Debug.Log("Game over");
                ShowPanelNewGame(false);
            }
        }
        
        private bool IsFlipped(int index)
        {
            return FlippedIndices.Contains(index);
        }

        private void HandleClick(int i)
        {
            if (IsFlipped(i) || !_canSelect || (_firstSelected != -1 && i == _firstSelected))
            {
                return;
            }
            var entryTransform = Items[i];
            var imageObj = entryTransform.Find("Image").GetComponent<Image>();
            imageObj.sprite = Resources.Load<Sprite>(String.Format("Sprites/{0}", Board[i]));
            imageObj.enabled = true;
            
            if (_firstSelected == -1)
            {
                _firstSelected = i;
            }
            
            else
            {
                _secondSelected = i;
                if (Board[_firstSelected] == Board[_secondSelected])
                {
                    audioSource.PlayOneShot(clipCorrect, volume);
                    FlippedIndices.Add(_firstSelected);
                    FlippedIndices.Add(_secondSelected);
                    _firstSelected = -1;
                    _secondSelected = -1;
                    UpdateCorrectStatistic();
                }
                else
                {
                    audioSource.PlayOneShot(clipWrong, volume);
                    UpdateWrongStatistic();
                    _canSelect = false;
                    Invoke("FlipSelectedImages", 0.3f);
                }
            }
            CheckGame();
        }

        private void UpdateWrongStatistic()
        {
            _turns -= 1;
        }
        
        private void UpdateCorrectStatistic()
        {
            _score += 1000;
            _correctPairs += 1;
            if (_correctPairs == _selectedLevel.Pairs && _turns >= 0)
            {
                _score += _turns * 100;
            }
        }
        

        public void FlipSelectedImages()
        {
            FlipImage(_firstSelected);
            FlipImage(_secondSelected);
            _firstSelected = -1;
            _secondSelected = -1;
            _canSelect = true;
        }

        private void FlipImage(int index)
        {
            var entryTransform = Items[index];
            var imageObj = entryTransform.Find("Image").GetComponent<Image>();
            imageObj.sprite = Resources.Load<Sprite>("Sprites/100");
        }

        private void FlipAll()
        {
            foreach (var entryTransform in Items)
            {
                var imageObj = entryTransform.Find("Image").GetComponent<Image>();
                imageObj.sprite = Resources.Load<Sprite>("Sprites/100");
            }
        }
        
        private ClickHandler clickHandler;
        private void Start()
        {
            // Create an instance of ClickHandler
            clickHandler = new ClickHandler();

            // Subscribe the HandleButtonClick method to the delegate
            ClickHandler.ButtonClickDelegate += HandleClick;
        }
    }
}