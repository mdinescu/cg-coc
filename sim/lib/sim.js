'use strict';

var _slicedToArray = function () { function sliceIterator(arr, i) { var _arr = []; var _n = true; var _d = false; var _e = undefined; try { for (var _i = arr[Symbol.iterator](), _s; !(_n = (_s = _i.next()).done); _n = true) { _arr.push(_s.value); if (i && _arr.length === i) break; } } catch (err) { _d = true; _e = err; } finally { try { if (!_n && _i["return"]) _i["return"](); } finally { if (_d) throw _e; } } return _arr; } return function (arr, i) { if (Array.isArray(arr)) { return arr; } else if (Symbol.iterator in Object(arr)) { return sliceIterator(arr, i); } else { throw new TypeError("Invalid attempt to destructure non-iterable instance"); } }; }();

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol && obj !== Symbol.prototype ? "symbol" : typeof obj; };

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

function Square(props) {
  var dot = props.selected ? dot = '*' : '';
  var className = "square" + (props.active ? ' active' : '');
  return React.createElement(
    'button',
    { className: className, onMouseEnter: function onMouseEnter() {
        return props.onMouseEnter();
      } },
    props.active
  );
}

var Board = function (_React$Component) {
  _inherits(Board, _React$Component);

  function Board() {
    _classCallCheck(this, Board);

    return _possibleConstructorReturn(this, (Board.__proto__ || Object.getPrototypeOf(Board)).apply(this, arguments));
  }

  _createClass(Board, [{
    key: 'renderSquare',
    value: function renderSquare(i) {
      var _this2 = this;

      return React.createElement(Square, { value: i, active: this.props.activeCell == i, onMouseEnter: function onMouseEnter() {
          return _this2.props.onMouseEnter(i);
        } });
    }
  }, {
    key: 'render',
    value: function render() {
      var rows = [];
      var rowCount = 21;
      var columnCount = 23;
      for (var i = 0; i < rowCount; i++) {
        var rowType = "board-row " + (i % 2 == 0 ? "even-row" : "odd-row");
        var columns = [];
        for (var j = 0; j < columnCount; j++) {
          columns.push(this.renderSquare(i * columnCount + j));
        }
        rows.push(React.createElement(
          'div',
          { className: rowType },
          columns
        ));
      }
      return React.createElement(
        'div',
        null,
        rows
      );
    }
  }]);

  return Board;
}(React.Component);

var Game = function (_React$Component2) {
  _inherits(Game, _React$Component2);

  function Game() {
    _classCallCheck(this, Game);

    var _this3 = _possibleConstructorReturn(this, (Game.__proto__ || Object.getPrototypeOf(Game)).call(this));

    _this3.state = {
      activeCell: 0,
      cellX: 0,
      cellY: 0
    };
    return _this3;
  }

  _createClass(Game, [{
    key: 'handleMouseEnter',
    value: function handleMouseEnter(i) {
      this.setState({
        activeCell: i,
        cellX: i % 23,
        cellY: parseInt(i / 23)
      });
    }
  }, {
    key: 'render',
    value: function render() {
      var _this4 = this;

      var status = 'Next player: X';
      var cellText = _typeof(this.state.activeCell) !== undefined ? "Active cell: " + this.state.activeCell + " -> " + this.state.cellX + "," + this.state.cellY : "--";
      return React.createElement(
        'div',
        { className: 'game' },
        React.createElement(
          'div',
          { className: 'game-board' },
          React.createElement(Board, { activeCell: this.state.activeCell, onMouseEnter: function onMouseEnter(i) {
              return _this4.handleMouseEnter(i);
            } })
        ),
        React.createElement(
          'div',
          { className: 'game-info' },
          React.createElement(
            'div',
            null,
            status
          ),
          React.createElement(
            'ol',
            null,
            cellText
          )
        )
      );
    }
  }]);

  return Game;
}(React.Component);

// ========================================

ReactDOM.render(React.createElement(Game, null), document.getElementById('container'));

function calculateWinner(squares) {
  var lines = [[0, 1, 2], [3, 4, 5], [6, 7, 8], [0, 3, 6], [1, 4, 7], [2, 5, 8], [0, 4, 8], [2, 4, 6]];
  for (var i = 0; i < lines.length; i++) {
    var _lines$i = _slicedToArray(lines[i], 3),
        a = _lines$i[0],
        b = _lines$i[1],
        c = _lines$i[2];

    if (squares[a] && squares[a] === squares[b] && squares[a] === squares[c]) {
      return squares[a];
    }
  }
  return null;
}