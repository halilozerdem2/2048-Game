using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
 using TMPro;
 
public class Block : MonoBehaviour
{
     public int Value;
     public Node Node;
     public Vector2 Pos=> transform.position;
     public Block MergingBlock;
     public bool Merging;
    [SerializeField] private SpriteRenderer _renderer;
    
    [SerializeField] private TextMeshPro _text;

    //[SerializeField] private Color32 _colorField;
   public void Init(BlockType type)
    {
      Value=type.Value;
     /*_colorField=type._Color; 
      _renderer.color = type._Color;
      Debug.Log(_renderer.color);
      Debug.Log(_colorField);
      */
      switch (Value)
      {
         case 2:
         _renderer.color=Color.cyan;
            break;
         case 4:
         _renderer.color=Color.yellow;
            break;
         case 8:
         _renderer.color=Color.red;
            break;
         case 16:
         _renderer.color=Color.blue;
            break;
         case 32:
         _renderer.color=Color.green;
            break;
         case 64:
         _renderer.color=Color.magenta;
            break;
         case 128:
         _renderer.color=Color.black;
            break;
         case 256:
         _renderer.color=Color.yellow;
            break;
         case 512:
         _renderer.color=Color.blue;
            break;
         case 1024:
         _renderer.color=Color.red;
            break;
         case 2048:
         _renderer.color=Color.cyan;
            break;
         case 4096:
         _renderer.color=Color.green;
            break;
         case 8192:
         _renderer.color=Color.blue;
            break;
         case 16384:
          _renderer.color=Color.black;
         break;
         
         default:
         _renderer.color=type._Color;
            break;
      }
      
       _text.text=type.Value.ToString();
 }
         public void SetBlock(Node node)
         { 
         
         if (Node!=null) Node.OccupiedBlock=null;
         Node=node;
         Node.OccupiedBlock=this;

         }
 public void MergeBlock(Block blockToMergeWith) //birleşecek blokları ayarlıyoruz
 {

    MergingBlock=blockToMergeWith;
    //bloklar birleştikten sonra kullanılan yeri tekrar işgal edilmemiş olarak atama yapıyoruz
    Node.OccupiedBlock=null; 
    
    //yanyana 3 adet aynı bloktan geldiği zaman soldaki ikisinin birleşmesi diğerininin birleşmemesini sağlamak için:
    MergingBlock.Merging=true;
 }
 //Bloğun birleşmeye müsait olup olmadığını kontrol ediyor.
   public bool CanMerge(int value)=>value==Value && !Merging&&MergingBlock==null;

    
}
