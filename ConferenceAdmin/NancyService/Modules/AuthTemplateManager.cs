﻿using NancyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NancyService.Modules
{
    public class AuthTemplateManager
    {
        public class templateQuery
        {

            public int authorizationID { get; set; }
            public string authorizationName { get; set; }
            public string authorizationDocument { get; set; }
          
        };

        public class TemplatePagingQuery
        {
            public int indexPage;
            public int maxIndex;
            public int rowCount;
            public List<templateQuery> results;

            public TemplatePagingQuery()
            {
                results = new List<templateQuery>();
            }
        };

        public List<templateQuery> getTemplates()
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var tempList = (from t in context.authorizationtemplates
                                    where t.deleted == false
                                    select new templateQuery
                                    {
                                        authorizationID = t.authorizationID,
                                        authorizationName = t.authorizationName,
                                        authorizationDocument = t.authorizationDocument,

                                    }).ToList();

                    return tempList;
                }
            }
            catch (Exception ex)
            {
                Console.Write("AdminManager.getTemplates error " + ex);
                return null;
            }


        }

        public TemplatePagingQuery getTemplates(int index)
        {
            TemplatePagingQuery page = new TemplatePagingQuery();

            try
            {
                int pageSize = 10;
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var tempList = (from t in context.authorizationtemplates
                                    where t.deleted == false
                                    select new templateQuery
                                    {
                                        authorizationID = t.authorizationID,
                                        authorizationName = t.authorizationName,
                                        authorizationDocument = t.authorizationDocument,

                                    }).OrderBy(x => x.authorizationID);

                    page.rowCount = tempList.Count();
                    if (page.rowCount > 0)
                    {
                        page.maxIndex = (int)Math.Ceiling(page.rowCount / (double)pageSize);
                        var templates = tempList.Skip(pageSize * index).Take(pageSize).ToList(); //Skip past rows and take new elements
                        page.results = templates;
                    }

                    return page;
                }
            }
            catch (Exception ex)
            {
                Console.Write("AdminManager.getTemplates(index) error " + ex);
                return null;
            }


        }

        public templateQuery addTemplate(templateQuery t)
        {
            authorizationtemplate template = new authorizationtemplate();
            template.authorizationID = t.authorizationID;
            template.authorizationName = t.authorizationName;
            template.authorizationDocument = t.authorizationDocument;
            template.deleted = false;
          
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    template.deleted = false;
                    context.authorizationtemplates.Add(template);
                    context.SaveChanges();
                    t.authorizationID = template.authorizationID;
                    return t;
                }
            }
            catch (Exception ex)
            {
                Console.Write("AdminManager.addTemplate error " + ex);
                return null;
            }

        }

        public bool updateTemplate(authorizationtemplate newTemp)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var template = (from temp in context.authorizationtemplates
                                    where temp.authorizationID == newTemp.authorizationID
                                    select temp).FirstOrDefault();
                    
                    template.authorizationName = newTemp.authorizationName;
                    template.authorizationDocument = newTemp.authorizationDocument;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("AdminManager.addTemplate error " + ex);
                return false;
            }

        }

        public bool deleteTemplate(int id)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {

                    //verificar que se quiere se de disable a todos los evaluation q los tiene o que simplemente no se pueda escoger desde la pantalla 
                    //donde se asignan los evaluation pero lo q ya lo tienen  se queden con las evaluaciones.

                    var template = (from temp in context.authorizationtemplates
                                    where temp.authorizationID == id
                                    select temp).FirstOrDefault();

                    template.deleted = true;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("AdminManager.deleteTemplate error " + ex);
                return false;
            }

        }

    }
}