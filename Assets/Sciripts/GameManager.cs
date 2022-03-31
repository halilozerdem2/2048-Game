using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // _nodes.Where()
using DG.Tweening; //dotween
 using System;  // ArgumentOutOfRangeException 
 using Random = UnityEngine.Random; // Random.value
 

public class GameManager : MonoBehaviour
{
   [SerializeField] private int _width=4;
   [SerializeField] private int _height=4;


   [SerializeField] private Node _nodePrefab;
   [SerializeField] private Block _blockPrefab;
   
   [SerializeField] private SpriteRenderer _boardPrefab;
   [SerializeField] private List<BlockType> _types;
   [SerializeField] private float _travelTime=0.2f;

   private List<Node> _nodes;
   private List<Block> _blocks;
   private GameState _state;
   private int _round;
   private BlockType GetBlockTypeByValue(int value)=>_types.First(t=>t.Value==value);


  
void Start() 
{
    ChangeState(GameState.GenerateLevel);

}
void Update() {
    if(_state!=GameState.WaitingInput) return;
    //hareket yönü kontrolü
    

    if(Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
    if(Input.GetKeyDown(KeyCode.RightArrow)) Shift(Vector2.right);
    if(Input.GetKeyDown(KeyCode.UpArrow)) Shift(Vector2.up);
    if(Input.GetKeyDown(KeyCode.DownArrow)) Shift(Vector2.down);

}
private void ChangeState(GameState newState)
{
    //Oyunu statelere bölüp stagelerde ilgili statlerde fonksiyonlar çağırlıyor.
    _state=newState;

    switch(newState)
    {
        case GameState.GenerateLevel:
        GenerateGrid();
        break;

        case GameState.SpawningBlocks:
        SpawnBlocks(_round ++ == 0 ? 2:1 );
        break;
        
        case GameState.WaitingInput:
    
        break;

        case GameState.Moving:
        break;

        case GameState.Win:
        break;

        case GameState.Lose:
        break;

        default:
        throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
    }
}

//4x4 lük Tahta yapısı for döngüsü yardımıyla hazırlanıyor.
   void GenerateGrid()
   {
       _round=0;
       _nodes=new List<Node>();
      _blocks=new List<Block>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node =Instantiate(_nodePrefab,new Vector3(x,y),Quaternion.identity);
                _nodes.Add(node);
            }
        }   

        var center=new Vector2((float) _width/2 -0.5f , (float) _height / 2 -0.5f);

         var board = Instantiate(_boardPrefab, center,Quaternion.identity);
         board.size=new Vector2(_width,_height);

         //kamera main konumu ayarlanıyor.   
         Camera.main.transform.position=new Vector3(center.x,center.y,-10);
         ChangeState(GameState.SpawningBlocks);
        
   }

     void SpawnBlocks(int amount)
    {
        var freeNodes=_nodes.Where(n=>n.OccupiedBlock==null).OrderBy(b=>Random.value).ToList();

        foreach (var node in freeNodes.Take(amount))
        {
             SpawnBlock(node,Random.value > 0.8f ? 4:2 );
        }

        if(freeNodes.Count()==1)
        {
            // oyunu kaybettin
            return ;
        }
       
     
            
        
        ChangeState(GameState.WaitingInput);
    }
    void SpawnBlock(Node node, int value)
        {   
            var block=Instantiate(_blockPrefab,node.Pos,Quaternion.identity);
            block.Init(GetBlockTypeByValue(value));
            block.SetBlock(node);
            _blocks.Add(block);
        }
        
        //Hareket (kayma) esnasında olacak olaylar belirleniyor.
    void Shift(Vector2 dir)
    {
        ChangeState(GameState.Moving);
        //bloklar önce x sonra y eksenine göre sıralanıyp listeye atanıyor.
        var orderedBlocks=_blocks.OrderBy(b=>b.Pos.x).ThenBy(b=>b.Pos.y).ToList();
        if(dir==Vector2.right || dir==Vector2.up) orderedBlocks.Reverse();
        
        foreach (var block in orderedBlocks)
        {   
            var next=block.Node;
            do
            {
                block.SetBlock(next);
                var possibleNode=GetNodeAtPosition(next.Pos+dir);
                    if(possibleNode!=null)
                    {//bir node olduğunu biliyoruz.
                    //birleşmeye uygunsa birleştir
                        if(possibleNode.OccupiedBlock!=null && possibleNode.OccupiedBlock.CanMerge(block.Value)) //Eğer hareket eden bloğun gitmesi gereken yer dolu ve yerini tutan bloğun üzerindeki değer ile aynı değere sahipse
                        {
                        block.MergeBlock(possibleNode.OccupiedBlock);

                        }
                        //değilse bu noktaya geçebilir miyi?
                        else if(possibleNode.OccupiedBlock==null) next=possibleNode;

                        //hiçbiri olmuyorsa döngüyü bitir
                        }

            } while (next!=block.Node);

        

        }    

        var sequence= DOTween.Sequence();

        foreach (var block in orderedBlocks)
        {
            var movePoint=block.MergingBlock !=null ? block.MergingBlock.Node.Pos: block.Node.Pos;
            sequence.Insert(0,block.transform.DOMove(movePoint,_travelTime));// bu sequence bütün blokların hareket ettikleri yerlerini tutuyor
        }
        sequence.OnComplete(()=>{
            foreach (var block in orderedBlocks.Where(b=>b.MergingBlock!=null))
            {
                MergeBlocks(block.MergingBlock,block);
            }
            ChangeState(GameState.SpawningBlocks);
        });


    }

    void MergeBlocks(Block baseBlock,Block mergingBlock)
    {
  
        SpawnBlock(baseBlock.Node,baseBlock.Value * 2);
        RemoveBlock(baseBlock);// bekleyen bloğu sil
        RemoveBlock(mergingBlock);// birleşen bloğu sil
       
       

    }
    void RemoveBlock(Block block)
    {
        
        _blocks.Remove(block) ;
        Destroy(block.gameObject);
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
     return _nodes.FirstOrDefault(n=>n.Pos==pos);   
    }

    
}
[System.Serializable]
public struct BlockType {
    public int Value;
    public Color _Color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose



}
