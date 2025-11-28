using Nanpure.Standard.Analyze;
using Nanpure.Standard.Core;
using Nanpure.Standard.InputSystem;
using Nanpure.Standard.Module;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nanpure.Battle
{
    public class BattleManager : MonoBehaviour, IBoardEventReceiver
    {
        [SerializeField] Sprite _highlight_img;
        List<GameObject> _sprites = new List<GameObject>();

        [Header("References")]
        [SerializeField] Board _board;
        [SerializeField] InputHandler _inputHandler;
        [SerializeField] TMP_Text _statusText;

        [Header("Battle Settings")]
        [SerializeField] float _attackTimeLimit = 10f;
        [SerializeField] int _maxHp = 3;
        [SerializeField] int _damagePerHit = 1;

        [Header("Highlight")]
        [SerializeField] Color _attackHighlightColor = Color.red;
        [SerializeField] Color _relatedHighlightColor = Color.yellow;
        [SerializeField] Color _defaultCellColor = Color.white;

        [Header("Solver Settings")]
        [SerializeField] bool _useNakedSingle = true;
        [SerializeField] bool _useHiddenSingle = true;
        [SerializeField] bool _useNakedPair = false;
        [SerializeField] bool _useHiddenPair = false;
        [SerializeField] bool _useNakedTriple = false;
        [SerializeField] bool _usePointingPair = false;
        [SerializeField] bool _useBoxLineReduction = false;
        [SerializeField] bool _useXWing = false;
        [SerializeField] bool _useXYWing = false;

        [Header("Debug (Runtime)")]
        [SerializeField] int _currentHp;
        [SerializeField] float _currentTimer;
        [SerializeField] string _currentAttackInfo;
        [SerializeField] bool _isBattleActive;

        AnalyzedMove _currentAttack;
        SolverOperation _currentOperation;
        List<Cell> _highlightedCells = new List<Cell>();

        public event Action<AnalyzedMove> OnAttackStart;
        public event Action<int> OnDamage;
        public event Action OnBattleEnd;

        public int CurrentHp => _currentHp;
        public float CurrentTimer => _currentTimer;
        public float AttackTimeLimit => _attackTimeLimit;
        public bool IsBattleActive => _isBattleActive;
        public AnalyzedMove CurrentAttack => _currentAttack;

        void OnEnable()
        {
            if (_inputHandler != null)
            {
                _inputHandler.onCellUpdate += OnCellUpdate;
            }
        }

        void OnDisable()
        {
            if (_inputHandler != null)
            {
                _inputHandler.onCellUpdate -= OnCellUpdate;
            }
        }

        void Update()
        {
            if (!_isBattleActive) return;

            UpdateStatusText();

            if (_currentAttack == null) return;

            _currentTimer -= Time.deltaTime;

            if (_currentTimer <= 0f)
            {
                OnTimeUp();
            }
        }

        public void StartBattle()
        {
            _currentHp = _maxHp;
            _isBattleActive = true;
            _currentAttack = null;
            _currentTimer = 0f;
            _currentAttackInfo = "";

            Log("Battle Started");
            GenerateNextAttack();
        }

        public void StopBattle()
        {
            _isBattleActive = false;
            _currentAttack = null;
            _currentTimer = 0f;
            _currentAttackInfo = "";
            ClearHighlights();
            UpdateStatusText();

            Log("Battle Stopped");
        }

        void OnCellUpdate(Cell cell)
        {
            if (!_isBattleActive) return;
            if (_currentAttack == null) return;

            int inputNumber = cell.State.DisplayNum;

            if (_currentAttack.Cell == cell && _currentAttack.TryGetNum(out int correctNum) && correctNum == inputNumber)
            {
                Log($"Attack Avoided: ({cell.Row}, {cell.Column}) = {inputNumber}");
                ClearHighlights();
                GenerateNextAttack();
            }
        }

        void OnTimeUp()
        {
            _currentHp -= _damagePerHit;
            Log($"Time Up! Damage: {_damagePerHit}, HP: {_currentHp}/{_maxHp}");
            OnDamage?.Invoke(_damagePerHit);

            ClearHighlights();

            if (_currentHp <= 0)
            {
                EndBattle();
                return;
            }

            GenerateNextAttack();
        }

        void GenerateNextAttack()
        {
            _currentAttack = null;
            _currentTimer = 0f;
            _currentAttackInfo = "Searching...";

            var logicTypes = GetActiveLogicTypes();
            if (logicTypes.Count == 0)
            {
                Log("No logic enabled");
                EndBattle();
                return;
            }

            _currentOperation = SolverResultAnalyzer.AnalyzeAsync(this, _board, logicTypes.ToArray());
            _currentOperation.Completed += OnAnalyzeCompleted;
        }

        void OnAnalyzeCompleted(SolverOperation operation)
        {
            if (!_isBattleActive) return;

            var results = operation.Result;
            AnalyzedMove placeableMove = null;

            foreach (var move in results)
            {
                if (move.CanPlace)
                {
                    placeableMove = move;
                    break;
                }
            }

            if (placeableMove == null)
            {
                Log("No attack found (puzzle complete or stuck)");
                EndBattle();
                return;
            }

            _currentAttack = placeableMove;
            _currentTimer = _attackTimeLimit;

            if (placeableMove.TryGetNum(out int num))
            {
                _currentAttackInfo = $"({placeableMove.Cell.Row}, {placeableMove.Cell.Column}) = {num} [{string.Join(", ", placeableMove.Techniques)}]";
            }

            ApplyHighlights();

            Log($"Attack Start: {_currentAttackInfo}");
            OnAttackStart?.Invoke(_currentAttack);
        }

        void EndBattle()
        {
            //_isBattleActive = false;
            //_currentAttack = null;
            //ClearHighlights();

            //if (_currentHp <= 0)
            //{
            //    _currentAttackInfo = "DEFEAT";
            //    Log("Battle End: Defeat");
            //}
            //else
            //{
            //    _currentAttackInfo = "VICTORY";
            //    Log("Battle End: Victory");
            //}

            //UpdateStatusText();
            //OnBattleEnd?.Invoke();
        }

        void ApplyHighlights()
        {
            if (_currentAttack == null) return;

            SetCellHighlight(_currentAttack.Cell, _attackHighlightColor);
            _highlightedCells.Add(_currentAttack.Cell);

            if (_currentAttack.RelatedCells != null)
            {
                foreach (var cell in _currentAttack.RelatedCells)
                {
                    //SetCellHighlight(cell, _relatedHighlightColor);
                    //_highlightedCells.Add(cell);
                }
            }
        }

        void ClearHighlights()
        {
            foreach(var go in _sprites)
            {
                Destroy(go);
            }

            //foreach (var cell in _highlightedCells)
            //{
            //    SetCellHighlight(cell, _defaultCellColor);
            //}
            //_highlightedCells.Clear();
        }

        void SetCellHighlight(Cell cell, Color color)
        {
            var go = new GameObject();
            go.transform.SetParent(cell.transform, false);
            go.transform.localScale = Vector3.one * 0.01f;
            var img = go.AddComponent<Image>();
            img.sprite = _highlight_img;
            img.raycastTarget = false;
            _sprites.Add(go);

            //var image = cell.GetComponentInChildren<Image>();
            //if (image != null)
            //{
            //    image.color = color;
            //}
        }

        void UpdateStatusText()
        {
            if (_statusText == null) return;

            if (!_isBattleActive)
            {
                _statusText.text = _currentAttackInfo;
                return;
            }

            string hpBar = new string('♥', _currentHp) + new string('♡', _maxHp - _currentHp);
            string timer = _currentAttack != null ? $"{_currentTimer:F1}s" : "---";

            _statusText.text = $"HP: {hpBar}\nTime: {timer}";
            //_statusText.text = $"HP: {hpBar}\nTime: {timer}\n{_currentAttackInfo}";
        }

        List<Type> GetActiveLogicTypes()
        {
            var types = new List<Type>();

            if (_useNakedSingle) types.Add(typeof(NakedSingleLogic));
            if (_useHiddenSingle) types.Add(typeof(HiddenSingleLogic));
            if (_useNakedPair) types.Add(typeof(NakedPairLogic));
            if (_useHiddenPair) types.Add(typeof(HiddenPairLogic));
            if (_useNakedTriple) types.Add(typeof(NakedTripleLogic));
            if (_usePointingPair) types.Add(typeof(PointingPairLogic));
            if (_useBoxLineReduction) types.Add(typeof(BoxLineReductionLogic));
            if (_useXWing) types.Add(typeof(XWingLogic));
            if (_useXYWing) types.Add(typeof(XYWingLogic));

            return types;
        }

        void Log(string message)
        {
            Debug.Log($"[BattleManager] {message}");
        }

        public void OnBoardEvent(Board board, BoardEvent boardEvent)
        {
            if (boardEvent == BoardEvent.Prepared)
                _board = board;

            StartBattle();
        }
    }
}