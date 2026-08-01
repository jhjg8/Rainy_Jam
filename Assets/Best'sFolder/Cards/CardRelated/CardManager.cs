using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour 
{
    public enum position { tavern, graveyard, hand, field, other };
    public static CardManager instance;

    public string[] names = {"Aarav",
                              "Abigail",
                              "Akira",
                              "Babatunde",
                              "Bjorn",
                              "Bohdana",
                              "Carlos",
                              "Chen",
                              "Chiara",
                              "Darius",
                              "Declan",
                              "Diya",
                              "Elias",
                              "Enkh",
                              "Enzo",
                              "Fatima",
                              "Felipe",
                              "Freja",
                              "Gabriel",
                              "Gael",
                              "Gisela",
                              "Habib",
                              "Hana",
                              "Heidi",
                              "Ibrahim",
                              "Ingrid",
                              "Ivan",
                              "Jabulani",
                              "Jari",
                              "Javier",
                              "Katarina",
                              "Kwame",
                              "Kyaw",
                              "Laszlo",
                              "Leilani",
                              "Lotte",
                              "Mateusz",
                              "Muna",
                              "Mustafa",
                              "Nam",
                              "Nia",
                              "Nuru",
                              "Olga",
                              "Omar",
                              "Oumar",
                              "Panya",
                              "Pavel",
                              "Pia",
                              "Qasim",
                              "Qian",
                              "Quique",
                              "Ranya",
                              "Reza",
                              "Rohan",
                              "Santiago",
                              "Sibusiso",
                              "Sofia",
                              "Tenzin",
                              "Thabo",
                              "Tiago",
                              "Ulises",
                              "Urte",
                              "Usain",
                              "Vahagn",
                              "Vasilis",
                              "Victor",
                              "Wael",
                              "Walid",
                              "Wira",
                              "Xhevat",
                              "Xin",
                              "Xolani",
                              "Yael",
                              "Yannick",
                              "Yasmin",
                              "Zahara",
                              "Zane",
                              "Zoran"};
    [SerializeField] private GameObject template;
    public Material Back;
    public List<Material> CardImage;

    public List<Suit> allsuits;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {

    }
    public GameObject createObj(int rank, Vector3 pos,List<Suit> suits)
    {
        GameObject n = Instantiate(template, new Vector3(0,0,0), Quaternion.identity);
        CardObj c = n.GetComponent<CardObj>();
        c.card.Rank = rank;
        c.Belong = pos;
        foreach (Suit suit in suits)
        {
            c.card.suits.Add(suit);
        }
        c.ResetPos();
        c.card.Addsuits();
        n.name = "Card";
        c.card.cardname = names[Random.Range(0, names.Length)];
        n.SetActive(true);
        return n;
    }
}
