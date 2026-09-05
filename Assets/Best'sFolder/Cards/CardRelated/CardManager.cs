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
    public Texture Back;
    public List<Texture> CardSpades;
    public List<Texture> CardDiamond;
    public List<Texture> CardHeart;
    public List<Texture> CardClub;

    public List<Suit> allsuits;
    private void Awake()
    {
        instance = this;
        if (template != null) template.SetActive(false); // hide reference prefab (was showing as extra 'Card' in scene)
    }
    private void Start()
    {
        foreach (Suit s in allsuits)
        {
            if (s != null) s.Enable();
        }
        if (template != null) template.SetActive(false);
    }
    public GameObject createObj(int rank, Vector3 pos,List<Suit> suits)
    {
        GameObject n = Instantiate(template, pos, Quaternion.identity);
        CardObj c = n.GetComponent<CardObj>();
        c.card.Rank = rank;
        c.Belong = pos;
        foreach (Suit suit in suits)
        {
            c.card.suits.Add(suit);
        }
        c.ResetPos();
        n.name = "Card";
        c.card.cardname = names[Random.Range(0, names.Length)];
        c.card.UseAble = true; // FIX: ensure usable (was showing "not usable" due to prefab default)
        
        var firstSuit = c.card.suits[0];
        Texture randomtexture = null;
        var mr = n.GetComponent<MeshRenderer>();
        if (firstSuit == CardManager.instance.allsuits[0] && CardClub.Count > 0)
        {
            randomtexture = CardClub[UnityEngine.Random.Range(0, CardClub.Count)];
            mr.materials[0].SetTexture("_mt", randomtexture);
        }
        else if (firstSuit == CardManager.instance.allsuits[1] && CardDiamond.Count > 0)
        {
            randomtexture = CardDiamond[UnityEngine.Random.Range(0, CardDiamond.Count)];
            mr.materials[0].SetTexture("_mt", randomtexture);
        }
        else if (firstSuit == CardManager.instance.allsuits[2] && CardHeart.Count > 0)
        {
            randomtexture = CardHeart[UnityEngine.Random.Range(0, CardHeart.Count)];
            mr.materials[0].SetTexture("_mt", randomtexture);
        }
        else if (firstSuit == CardManager.instance.allsuits[3] && CardSpades.Count > 0)
        {
            randomtexture = CardSpades[UnityEngine.Random.Range(0, CardSpades.Count)];
            mr.materials[0].SetTexture("_mt", randomtexture);
        }
        c.M = randomtexture;
        n.SetActive(true);
        return n;
    }
}
