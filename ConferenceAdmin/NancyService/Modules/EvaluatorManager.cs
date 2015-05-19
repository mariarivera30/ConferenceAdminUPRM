﻿using NancyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace NancyService.Modules
{
    public class EvaluatorManager
    {
        //ccwicEmail
        string ccwicEmail = "ccwictest@gmail.com";
        string ccwicEmailPass = "ccwic123456789";
        string testEmail = "heidi.negron1@upr.edu";

        //Code by: Heidi Negron
        public EvaluatorManager()
        {

        }

        //Verify the specified email has an accoun in conference admin.
        //Returns true if a match is made, false otherwise
        public bool checkNewEvaluator(String email)
        {
            try
            {

                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var result = (from s in context.users
                                  where s.membership.email == email
                                  select s.userID).Count();

                    if (result > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Write("EvaluatorManager.checkNewEvaluator error " + ex);
                return false;
            }
        }

        //Get list of evaluators with an 'Accepted' or 'Rejected' status
        //Returns userID, first name, last name, email and its evaluator status
        public EvaluatorPagingQuery getEvaluatorList(int index, int id)
        {
            EvaluatorPagingQuery e = new EvaluatorPagingQuery();

            try
            {

                using (conferenceadminContext context = new conferenceadminContext())
                {
                    int pageSize = 10;
                    //Get list of evaluators
                    var query = context.users.Where(evaluator => (evaluator.evaluatorStatus == "Accepted" || evaluator.evaluatorStatus == "Rejected") && evaluator.userID != id && context.claims.Where(x => x.userID == evaluator.userID && x.deleted != true && (x.privilege.privilegesID == 1 || x.privilege.privilegesID == 3 || x.privilege.privilegesID == 5)).Select(x => x.userID).Count() == 0).Select(evaluator => new EvaluatorQuery
                    {
                        userID = (long)evaluator.userID,
                        firstName = evaluator.firstName,
                        lastName = evaluator.lastName,
                        email = evaluator.membership.email,
                        acceptanceStatus = evaluator.evaluatorStatus

                    }).OrderBy(x => x.email);

                    e.rowCount = query.Count();
                    //Paging -> Filter results to obtain 10 results per page (index references to page number)
                    if (e.rowCount > 0)
                    {
                        e.maxIndex = (int)Math.Ceiling(e.rowCount / (double)pageSize);
                        var evaluators = query.Skip(pageSize * index).Take(pageSize).ToList(); //Skip past rows and take new elements
                        e.results = evaluators;
                    }

                    return e;
                }
            }
            catch (Exception ex)
            {
                Console.Write("EvaluatorManager.getEvaluatorList error " + ex);
                return null;
            }
        }

        //Get list of evaluators with an 'Pending' status
        //Returns userID, first name, last name, email and its evaluator status
        public EvaluatorPagingQuery getPendingList(int index)
        {
            EvaluatorPagingQuery e = new EvaluatorPagingQuery();

            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    int pageSize = 10;
                    //Get list of pending applications
                    var query = context.users.Where(evaluator => evaluator.evaluatorStatus == "Pending").Select(evaluator => new EvaluatorQuery
                    {
                        userID = (long)evaluator.userID,
                        firstName = evaluator.firstName,
                        lastName = evaluator.lastName,
                        email = evaluator.membership.email,
                        acceptanceStatus = evaluator.evaluatorStatus

                    }).OrderBy(x => x.email);

                    e.rowCount = query.Count();
                    //Paging -> Filter results to obtain 10 results per page (index references to page number)
                    if (e.rowCount > 0)
                    {
                        e.maxIndex = (int)Math.Ceiling(e.rowCount / (double)pageSize);
                        var pending = query.Skip(pageSize * index).Take(pageSize).ToList(); //Skip past rows and take new elements
                        e.results = pending;
                    }

                    return e;
                }
            }
            catch (Exception ex)
            {
                Console.Write("EvaluatorManager.getPendingList error " + ex);
                return null;
            }
        }

        //Update the status of an evaluator
        public bool updateAcceptanceStatus(EvaluatorQuery e)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var updateUser = (from s in context.users
                                      where s.userID == e.userID
                                      select s).FirstOrDefault();
                    if (updateUser != null)
                    {
                        if (e.acceptanceStatus == "Rejected")
                        {
                            //Remove from claim table
                            var updateClaim = (from s in context.claims
                                               where s.userID == e.userID && s.privilege.privilegestType == "Evaluator"
                                               select s).FirstOrDefault();

                            if (updateClaim != null)
                            {
                                updateClaim.deleted = true;
                            }

                            //Remove from evaluator table
                            var updateEvaluator = (from s in context.evaluators
                                                   where s.userID == e.userID
                                                   select s).FirstOrDefault();

                            if (updateEvaluator != null)
                            {
                                //list of the the submissions assigned to the evaluator with ID evaluatorID
                                List<evaluatiorsubmission> evaluatorAssignments = context.evaluatiorsubmissions.Where(c => c.evaluatorID == updateEvaluator.evaluatorsID && c.deleted == false).ToList();

                                foreach (var assignment in evaluatorAssignments)
                                {
                                    if (assignment.evaluationsubmitteds.FirstOrDefault() == null)//if no evaluation was submitted then delete the assignment
                                    {
                                        assignment.deleted = true;
                                    }
                                }

                                updateEvaluator.deleted = true;
                            }
                            //Update status
                            updateUser.evaluatorStatus = e.acceptanceStatus;

                            //Send reject email
                            try { sendRejectConfirmation(updateUser.firstName, updateUser.membership.email, "Rejected"); }
                            catch (Exception ex)
                            {
                                Console.Write("AdminManager.sendRejectEvaluatorEmail error " + ex);
                                return false;
                            }
                        }

                        else if (e.acceptanceStatus == "Accepted")
                        {
                            //Give evaluator privilege to user
                            this.addEvaluator(updateUser.membership.email);
                            //Send confirmation email
                            try { sendAcceptConfirmation(updateUser.firstName, updateUser.membership.email, "Accepted"); }
                            catch (Exception ex)
                            {
                                Console.Write("AdminManager.sendRejectEvaluatorEmail error " + ex);
                                return false;
                            }
                        }

                        context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write("EvaluatorManager.updateAcceptanceStatus error " + ex);
                return false;
            }
        }

        //Give an evaluator the "Accepted" status
        public EvaluatorQuery addEvaluator(String email)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var e = (from u in context.users
                             where u.membership.email == email
                             select u).FirstOrDefault();
                    if (e != null)
                    {
                        //Check if user has been an evaluator before
                        var check = (from s in context.evaluators
                                     where s.userID == e.userID
                                     select s).FirstOrDefault();

                        if (check != null)
                        {
                            //Update status to Accepted
                            check.deleted = false;
                            var claims = (from s in context.claims
                                          where s.userID == e.userID && s.privilege.privilegestType == "Evaluator"
                                          select s).FirstOrDefault();
                            if (claims != null)
                            {
                                claims.deleted = false;
                            }
                            else
                            {
                                //Add claim 
                                claim newClaim = new claim();
                                newClaim.privilegesID = 4;
                                newClaim.deleted = false;
                                newClaim.userID = e.userID;
                                context.claims.Add(newClaim);
                            }

                            e.evaluatorStatus = "Accepted";
                        }

                        else
                        {
                            //Change status in table user
                            EvaluatorQuery newEvaluator = new EvaluatorQuery();
                            newEvaluator.userID = e.userID;
                            newEvaluator.firstName = e.firstName;
                            newEvaluator.lastName = e.lastName;
                            newEvaluator.email = email;
                            newEvaluator.acceptanceStatus = e.evaluatorStatus;

                            //Add claim 
                            claim newClaim = new claim();
                            newClaim.privilegesID = 4;
                            newClaim.deleted = false;
                            newClaim.userID = e.userID;
                            context.claims.Add(newClaim);

                            //Add evaluator
                            evaluator newEva = new evaluator();
                            newEva.userID = e.userID;
                            newEva.deleted = false;
                            context.evaluators.Add(newEva);

                            e.evaluatorStatus = "Accepted";

                            context.SaveChanges();
                            return newEvaluator;
                        }

                        context.SaveChanges();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.Write("EvaluatorManager.addEvaluator error " + ex);
                return null;
            }
        }

        //Search evaluators that contain the specified criteria
        //Returns userID, first name, last name, email and its evaluator status
        public EvaluatorPagingQuery searchEvaluators(int index, String criteria)
        {
            EvaluatorPagingQuery e = new EvaluatorPagingQuery();

            try
            {

                using (conferenceadminContext context = new conferenceadminContext())
                {
                    int pageSize = 10;
                    //Get all evaluators applications 
                    var query = context.users.Where(evaluator => ((evaluator.firstName.ToLower() + " " + evaluator.lastName.ToLower()).Contains(criteria.ToLower()) || evaluator.membership.email.ToLower().Contains(criteria.ToLower())) && (evaluator.evaluatorStatus == "Accepted" || evaluator.evaluatorStatus == "Rejected" || evaluator.evaluatorStatus == "Pending")).Select(evaluator => new EvaluatorQuery
                    {
                        userID = (long)evaluator.userID,
                        firstName = evaluator.firstName,
                        lastName = evaluator.lastName,
                        email = evaluator.membership.email,
                        acceptanceStatus = evaluator.evaluatorStatus

                    }).OrderBy(x => x.email);

                    e.rowCount = query.Count();
                    //Paging -> Filter results to 10 records per index (page) 
                    if (e.rowCount > 0)
                    {
                        e.maxIndex = (int)Math.Ceiling(e.rowCount / (double)pageSize);
                        var evaluators = query.Skip(pageSize * index).Take(pageSize).ToList(); //Skip past rows and take new elements
                        e.results = evaluators;
                    }

                    return e;
                }
            }
            catch (Exception ex)
            {
                Console.Write("EvaluatorManager.searchEvaluators error " + ex);
                return null;
            }
        }

        //Send confirmation email notifying of status
        private void sendAcceptConfirmation(String name, String email, String p)
        {
            MailAddress ccwic = new MailAddress(ccwicEmail);
            MailAddress user = new MailAddress(email);
            MailMessage mail = new System.Net.Mail.MailMessage(ccwic, user);

            String closing = " \r\nThank you.\r\nCCWiC Administration";

            mail.Subject = "Caribbean Celebration of Women in Computing- Evaluator Information";
            mail.Body = "Greetings "+ name+",\r\n \r\nYour evaluator status has been updated to: " + p + ". You can now access Administrator Settings by login in ConferenceAdmin.\r\n" + closing;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            smtp.Credentials = new NetworkCredential(
                ccwicEmail, ccwicEmailPass);
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }
        private void sendRejectConfirmation(String name, String email, String p)
        {
            MailAddress ccwic = new MailAddress(ccwicEmail);
            MailAddress user = new MailAddress(email);
            MailMessage mail = new System.Net.Mail.MailMessage(ccwic, user);

            String closing = " \r\nThank you.\r\nCCWiC Administration";

            mail.Subject = "Caribbean Celebration of Women in Computing- Evaluator Information";
            mail.Body = "Greetings " + name + ",\r\n \r\nYour evaluator status has been updated to: " + p + ".\r\n" + closing;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            smtp.Credentials = new NetworkCredential(
                ccwicEmail, ccwicEmailPass);
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }

    }

    public class EvaluatorQuery
    {
        public long userID;
        public long evaluatorID;
        public String firstName;
        public String lastName;
        public String email;
        public String acceptanceStatus;
        public String optionStatus = "Accept";

        public EvaluatorQuery()
        {
        }
    }

    public class EvaluatorPagingQuery
    {
        public int indexPage;
        public int maxIndex;
        public int rowCount;
        public List<EvaluatorQuery> results;
    }
}