using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OneID
{
    public class OneIDStudent
    {
        private string _name;
        private string _showName;
        private int _age;
        private bool _isSelect;
        private bool _isPKSelect;
        private bool _isSignIn;
        private int _score;
        private bool _isMagicSqareGiftSelect;
                
        public OneIDStudent(string name,string showName)
        {
            _name = name;
            _isSelect = false;
            _isPKSelect = false;
            _isSignIn = false;
            _isMagicSqareGiftSelect = false;
            _showName = showName;
            switch (name)
            {
                // case "1":
                // case "2":
                //     _score = 0;
                //     break;
                case "4":
                    _score = 1000;
                    break;
                case "5":
                    _score = 600;
                    break;
                case "7":
                    _score = 800;
                    break;
                case "8":
                    _score = 500;
                    break;
                default:
                    _score = 0;
                    break;
            }
            
        }

        public string Name
        {
            get => _name;
        }

        public bool IsSignIn
        {
            get => _isSignIn;
            set => _isSignIn = value;
        }

        public bool IsSelect
        {
            get => _isSelect;
            set => _isSelect = value;
        }

        public bool IsPKSelect
        {
            get => _isPKSelect;
            set => _isPKSelect = value;
        }

        public bool IsMagicSqareGiftSelect
        {
            get => _isMagicSqareGiftSelect;
            set => _isMagicSqareGiftSelect = value;
        }

        public string ShowName => _showName;

        public void AddScore(int score)
        {
            _score += score;
            if (_score <= 0)
                _score = 0;
        }

        public void MinusScore(int score)
        {
            _score -= score;
            if (_score <= 0)
                _score = 0;
        }

        public int GetScore()
        {
            return _score;
        }
    }
}