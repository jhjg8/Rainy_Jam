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
    public List<Texture> CardSpades;
    public List<Texture> CardDiamond;
    public List<Texture> CardHeart;
    public List<Texture> CardClub;

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
        GameObject n = Instantiate(template, new Vector3(0,180,0), Quaternion.identity);
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
        int suitsCount = 1;
        bool isbreake = false;
        while (!isbreake)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            if (chance < Mathf.Clamp((25 + EncounterManager.instance.DificaultyLevel), 25, 50))
            {
                suitsCount++;
            }
            else if (suitsCount >= 6)
            {
                isbreake = true;
            }
            else
            {
                isbreake = true;
            }
        }
        for (int i = 0; i < suitsCount; i++)
        {
            if (i == 0)
                c.card.suits.Add(CardManager.instance.allsuits[UnityEngine.Random.Range(0, 4)]);

            else
                c.card.suits.Add(CardManager.instance.allsuits[UnityEngine.Random.Range(4, CardManager.instance.allsuits.Count)]);
        }
        var firstSuit = c.card.suits[0];
        if (firstSuit == CardManager.instance.allsuits[0])
        {
            n.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", CardClub[UnityEngine.Random.Range(0, CardClub.Count)]);
        }
        else if (firstSuit == CardManager.instance.allsuits[1])
        {
            n.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", CardDiamond[UnityEngine.Random.Range(0, CardDiamond.Count)]);
        }
        else if (firstSuit == CardManager.instance.allsuits[2])
        {
            n.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", CardHeart[UnityEngine.Random.Range(0, CardHeart.Count)]);
        }
        else if (firstSuit == CardManager.instance.allsuits[3])
        {
            n.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", CardSpades[UnityEngine.Random.Range(0, CardSpades.Count)]);
        }
        n.SetActive(true);
        return n;
    }
}
