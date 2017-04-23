function Square(props) {
  let dot = (props.selected) ? dot = '*' : '';
  let className = "square" + (props.active ? ' active': '');    
  return (    
    <button className={className} onMouseEnter={() => props.onMouseEnter()}>
      {props.active}
    </button>
  );
}

class Board extends React.Component {
  renderSquare(i) {
    return <Square value={i} active={this.props.activeCell==i} onMouseEnter={() => this.props.onMouseEnter(i)}/>;
  }
  render() {    
    const rows = [];
    const rowCount = 21;
    const columnCount = 23;    
    for (let i = 0; i < rowCount; i++) {
      const rowType = "board-row " + ((i % 2 == 0) ? "even-row" : "odd-row");
      const columns = [];
      for(let j = 0; j < columnCount; j++) {
        columns.push(this.renderSquare(i*columnCount + j));
      }
      rows.push(
          <div className={rowType}>        
            {columns}
          </div>
        );      
    }
    return (
      <div>        
        {rows}      
      </div>
    );
  }
}

class Game extends React.Component {
  constructor() {
    super();
    this.state = {
      activeCell: 0,
      cellX: 0,
      cellY: 0
    };
  }
  handleMouseEnter(i) {
    this.setState({      
      activeCell: i,
      cellX: i % 23,
      cellY: parseInt(i / 23)
    });
  }
  render() {
    const status = 'Next player: X';
    const cellText = typeof(this.state.activeCell) !== undefined ?
               ("Active cell: " + this.state.activeCell + " -> " + this.state.cellX + "," + this.state.cellY) :
                "--";
    return (
      <div className="game">
        <div className="game-board">
          <Board activeCell={this.state.activeCell} onMouseEnter={(i) => this.handleMouseEnter(i)} />
        </div>
        <div className="game-info">
          <div>{status}</div>
          <ol>{cellText}</ol>
        </div>
      </div>
    );
  }
}

// ========================================

ReactDOM.render(
  <Game />,
  document.getElementById('container')
);

function calculateWinner(squares) {
  const lines = [
    [0, 1, 2],
    [3, 4, 5],
    [6, 7, 8],
    [0, 3, 6],
    [1, 4, 7],
    [2, 5, 8],
    [0, 4, 8],
    [2, 4, 6],
  ];
  for (let i = 0; i < lines.length; i++) {
    const [a, b, c] = lines[i];
    if (squares[a] && squares[a] === squares[b] && squares[a] === squares[c]) {
      return squares[a];
    }
  }
  return null;
}
