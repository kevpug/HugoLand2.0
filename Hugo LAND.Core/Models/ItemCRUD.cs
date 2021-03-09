﻿using Hugo_LAND.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hugo_LAND.Core.Models
{
    public static class ItemCRUD
    {
        public static void CreerItem(Item item)
        {
            using (HugoLANDContext context = new HugoLANDContext())
            {
                context.Mondes.Find(item.Monde.Id).Items.Add(new Item()
                {
                    Nom = item.Nom,
                    Description = item.Description,
                    x = item.x,
                    y = item.y,
                    ImageId = item.ImageId,
                    Hero = item.Hero,
                });
                context.SaveChanges();
            }
        }

        public static void RamasserItem(int idItem, int idHero)
        {
            using (HugoLANDContext context = new HugoLANDContext())
            {
                Hero hero = context.Heros.Find(idHero);
                Item item = context.Items.Find(idItem);
                item.x = null;
                item.y = null;
                item.Hero = hero;
                hero.Items.Add(item);
                context.SaveChanges();
            }
        }

        public static void SupprimerItem(Item item)
        {
            using (HugoLANDContext context = new HugoLANDContext())
            {
                context.Items.Remove(context.Items.Find(item.Id));
                context.SaveChanges();
            }
        }

        public static void ViderListeItems(Monde monde)
        {
            using (HugoLANDContext context = new HugoLANDContext())
            {
                foreach (var i in monde.Items)
                    context.Items.Remove(context.Items.Find(i.Id));
                context.SaveChanges();
            }
        }

        public static void AjouterPlusieursItems(Monde monde, List<Item> liste)
        {
            using (HugoLANDContext context = new HugoLANDContext())
            {
                foreach (var item in liste)
                    context.Mondes.Find(monde.Id).Items.Add(new Item()
                    {
                        Nom = item.Nom,
                        Description = item.Description,
                        x = item.x,
                        y = item.y,
                        ImageId = item.ImageId,
                        Hero = item.Hero,
                    });
                context.SaveChanges();
            }
        }

        public static void ModifierQuantiteItem(int idItem, int idHero, int quantite) //On s'excuse 
        {
            if (quantite < 0)
                throw new Exception("ErreurQuantitéNégative");

            using (HugoLANDContext context = new HugoLANDContext())
            {
                Hero hero = context.Heros.Find(idHero);
                Item item = context.Items.Find(idItem);
                int nombreItems = hero.Items.Where(it => it.Nom == item.Nom && it.Hero.Id == idHero && it.Monde.Id == item.Monde.Id).Count();
                int nombreDiff = Math.Abs(quantite - nombreItems);
                if (nombreDiff > 0)
                {
                    if (nombreItems < quantite) //En ajouter
                    {
                        for (int i = 0; i < nombreDiff; i++)
                        {
                            CreerItem(new Item()
                            {
                                Nom = item.Nom,
                                Description = item.Description,
                                x = item.x,
                                y = item.y,
                                ImageId = item.ImageId,
                                Monde = item.Monde
                            });
                            RamasserItem(context.Items.ToList().LastOrDefault(it => it.Nom == item.Nom).Id, idHero);
                        }
                    }
                    else if (nombreItems > quantite)  // En retirer
                    {
                        for (int i = 0; i < nombreDiff; i++)
                        {
                            Item itemRemove = context.Items.ToList().LastOrDefault(it => it.Nom == item.Nom && it.Hero.Id == idHero && it.Monde.Id == item.Monde.Id);
                            hero.Items.Remove(itemRemove);
                            context.Items.Remove(context.Items.Find(itemRemove.Id));
                        }
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
