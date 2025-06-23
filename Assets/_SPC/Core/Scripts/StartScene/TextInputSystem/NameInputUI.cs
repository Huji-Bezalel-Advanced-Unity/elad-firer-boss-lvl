using System;
using System.Collections.Generic;
using _SPC.Core.Scripts.StartScene.TextInputSystem;
using _SPC.Core.Scripts.Utils;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _SPC.Core.Scripts.Text
{
    public class NameInputUI : MonoBehaviour
    {
        
        public static readonly string PlayerPrefsName = "Nickname";
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _nameDisplayText;
        [SerializeField] private Transform _gridContainer;
        [SerializeField] private GameObject _gridButtonPrefab;
        [SerializeField] private Color _highlightColor = Color.yellow;
        [SerializeField] private Color _defaultColor = Color.white;

        [Header("Settings")]
        [SerializeField] private int _maxNameLength = 10;
        [SerializeField] private float _moveCooldown = 0.2f;

        private NameInputGrid _nameInputGrid;
        private readonly Dictionary<Vector2Int, Image> _gridButtons = new Dictionary<Vector2Int, Image>();
        private string _currentName = "";
        private Image _currentSelectionImage;

        public event Action<string> OnSubmitName;
        private void Awake()
        {
            _nameInputGrid = new NameInputGrid(_moveCooldown);
        }

        private void OnEnable()
        {
            _nameInputGrid.Enable();
            _nameInputGrid.OnSelectionChanged += HandleSelectionChanged;
            _nameInputGrid.OnCharacterSelected += HandleCharacterSelected;
            _nameInputGrid.OnBackspace += HandleBackspace;
            _nameInputGrid.OnSubmit += HandleSubmit;
        }

        private void OnDisable()
        {
            _nameInputGrid.Disable();
            _nameInputGrid.OnSelectionChanged -= HandleSelectionChanged;
            _nameInputGrid.OnCharacterSelected -= HandleCharacterSelected;
            _nameInputGrid.OnBackspace -= HandleBackspace;
            _nameInputGrid.OnSubmit -= HandleSubmit;
        }

        private void Start()
        {
            _currentName = PlayerPrefs.GetString(PlayerPrefsName,"");
            UpdateNameDisplay();
            _nameDisplayText.DOFade(1f, 2f);
            GenerateGrid();
            _nameInputGrid.InvokeInitialSelection();
        }

        private void GenerateGrid()
        {
            var gridLayoutGroup = _gridContainer.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup == null)
            {
                Debug.LogError("NameInputUI requires a GridLayoutGroup on the GridContainer to function correctly.", _gridContainer);
                return;
            }
            string[] gridLayout = _nameInputGrid.CharacterGrid;
            int buttonIndex = 0;
            for (int y = 0; y < gridLayout.Length; y++)
            {
                for (int x = 0; x < gridLayout[y].Length; x++)
                {
                    var buttonGO = Instantiate(_gridButtonPrefab, _gridContainer);

                    var canvasGroup = buttonGO.GetComponent<CanvasGroup>() ?? buttonGO.AddComponent<CanvasGroup>();
                    canvasGroup.alpha = 0f;
                    float delay = buttonIndex * 0.05f;
                    canvasGroup.DOFade(1f, 0.4f).SetDelay(delay);

                    var buttonImage = buttonGO.GetComponent<Image>();
                    var buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();

                    string character = gridLayout[y][x].ToString();

                    if (character == "<") character = "BACK";
                    
                    else if (character == ">") character = "END";
                    
                    buttonText.text = character;
                    
                    _gridButtons.Add(new Vector2Int(x, y), buttonImage);
                    buttonIndex++;
                }
            }
        }

        private void HandleSelectionChanged(Vector2Int newSelection)
        {
            if (_currentSelectionImage != null)
            {
                _currentSelectionImage.color = _defaultColor;
            }

            if (_gridButtons.TryGetValue(newSelection, out var newImage))
            {
                newImage.color = _highlightColor;
                _currentSelectionImage = newImage;
            }
        }

        private void HandleCharacterSelected(char character)
        {
            if (_currentName.Length < _maxNameLength)
            {
                _currentName += character;
                UpdateNameDisplay();
            }
        }

        private void HandleBackspace()
        {
            if (_currentName.Length > 0)
            {
                _currentName = _currentName.Substring(0, _currentName.Length - 1);
                UpdateNameDisplay();
            }
        }

        private void UpdateNameDisplay()
        {
            _nameDisplayText.text = _currentName;
        }

        private void HandleSubmit()
        {
            if(_currentName.Equals("")) return;
            PlayerPrefs.SetString(PlayerPrefsName, _currentName);
            OnSubmitName?.Invoke(_currentName);
        }
    }
} 