﻿using NancyService.Models;
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace NancyService.Modules
{
    public class WebManager
    {
        string ccwicEmail = "ccwictest@gmail.com";
        string ccwicEmailPass = "ccwic123456789";
        string testEmail = "heidi.negron1@upr.edu";

        //By: Heidi Negron
        public WebManager()
        {

        }

        //Get the content of an element from the web interface
        //Returns elementID, and its content
        public ContentQuery getInterfaceElement(String element)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    ContentQuery webInterface = new ContentQuery();
                    //Select element and save its content
                    webInterface = context.interfaceinformations.Where(inter => inter.attribute == element).Select(inter => new ContentQuery
                    {
                        interfaceID = (int)inter.interfaceID,
                        content = inter.content

                    }).FirstOrDefault();

                    if (webInterface.content == null)
                    {
                        webInterface.content = "";
                    }

                    return webInterface;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.get" + element + " error " + ex);
                return null;
            }

        }

        //Get the deadlines of an specified submission
        public ContentQuery getDeadlineElement(String element)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    ContentQuery webInterface = new ContentQuery();

                    //Get element and save its content (date)
                    webInterface = context.submissiontypes.Where(inter => inter.name == element).Select(inter => new ContentQuery
                    {
                        content = inter.deadline

                    }).FirstOrDefault();

                    if (webInterface.content == null)
                    {
                        webInterface.content = "";
                    }

                    return webInterface;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.get" + element + " error " + ex);
                return null;
            }

        }

        //Get Home section of the Website
        public HomeQuery getHome()
        {
            try
            {
                HomeQuery home = new HomeQuery();
                home.homeMainTitle = this.getInterfaceElement("homeMainTitle").content;
                home.homeParagraph1 = this.getInterfaceElement("homeParagraph1").content;

                return home;

            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getHome error " + ex);
                return null;
            }
        }

        //Get Image found in the Home section of the website
        public HomeQuery getHomeImage()
        {
            try
            {
                HomeQuery home = new HomeQuery();
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element and save its content
                    var img = (from s in context.interfacedocuments
                               where s.attibuteName == "homeImage"
                               select s).FirstOrDefault();

                    if (img != null)
                    {
                        home.image = img.content;
                    }
                }

                return home;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getHomeImage error " + ex);
                return null;
            }
        }

        //Update content of the Home section of the website
        public bool saveHome(HomeQuery newHome)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element
                    var homeMainTitle = (from s in context.interfaceinformations
                                         where s.attribute == "homeMainTitle"
                                         select s).FirstOrDefault();
                    //Save new content
                    if (homeMainTitle != null)
                        homeMainTitle.content = newHome.homeMainTitle;

                    var homeParagraph1 = (from s in context.interfaceinformations
                                          where s.attribute == "homeParagraph1"
                                          select s).FirstOrDefault();

                    if (homeParagraph1 != null)
                    {
                        newHome.homeParagraph1 = newHome.homeParagraph1 == null ? "" : newHome.homeParagraph1;
                        homeParagraph1.content = newHome.homeParagraph1;
                    }

                    //Get image element
                    var img = (from s in context.interfacedocuments
                               where s.attibuteName == "homeImage"
                               select s).FirstOrDefault();

                    //Save new content
                    if (img != null && newHome.image != null && newHome.image != "")
                        img.content = newHome.image;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.saveHome error " + ex);
                return false;
            }
        }

        //Remove Uploaded File
        public bool removeFile(String src)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var img = (from s in context.interfacedocuments
                               where s.attibuteName == src
                               select s).FirstOrDefault();
                    //Empty element content
                    if (img != null)
                        img.content = "";

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.removeImage error " + ex);
                return false;
            }
        }

        //Get content found in the Venue section of the website
        public VenueQuery getVenue()
        {
            try
            {
                VenueQuery venue = new VenueQuery();
                venue.venueParagraph1 = this.getInterfaceElement("venueParagraph1").content;
                venue.venueTitleBox = this.getInterfaceElement("venueTitleBox").content;
                venue.venueParagraphContentBox = this.getInterfaceElement("venueParagraphContentBox").content;

                return venue;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getVenue error " + ex);
                return null;
            }
        }

        //Update content of the Venue section of the website
        public bool saveVenue(VenueQuery newVenue)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element (occurs for each element)
                    var venueParagraph1 = (from s in context.interfaceinformations
                                           where s.attribute == "venueParagraph1"
                                           select s).FirstOrDefault();

                    //Save content (occurs for each element)
                    if (venueParagraph1 != null)
                    {
                        newVenue.venueParagraph1 = newVenue.venueParagraph1 == null ? "" : newVenue.venueParagraph1;
                        venueParagraph1.content = newVenue.venueParagraph1;
                    }

                    var venueTitleBox = (from s in context.interfaceinformations
                                         where s.attribute == "venueTitleBox"
                                         select s).FirstOrDefault();

                    if (venueTitleBox != null)
                        venueTitleBox.content = newVenue.venueTitleBox;


                    var venueParagraphContentBox = (from s in context.interfaceinformations
                                                    where s.attribute == "venueParagraphContentBox"
                                                    select s).FirstOrDefault();

                    if (venueParagraphContentBox != null)
                    {
                        newVenue.venueParagraphContentBox = newVenue.venueParagraphContentBox == null ? "" : newVenue.venueParagraphContentBox;
                        venueParagraphContentBox.content = newVenue.venueParagraphContentBox;
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveVenue error " + ex);
                return false;
            }
        }

        //Get content found in the Contact section of the website
        public ContactQuery getContact()
        {
            try
            {
                ContactQuery contact = new ContactQuery();
                contact.contactName = this.getInterfaceElement("contactName").content;
                contact.contactPhone = this.getInterfaceElement("contactPhone").content;
                contact.contactEmail = this.getInterfaceElement("contactEmail").content;
                contact.contactAdditionalInfo = this.getInterfaceElement("contactAdditionalInfo").content;

                return contact;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getContact error " + ex);
                return null;
            }
        }

        //Update content of the Contact section of the website
        public bool saveContact(ContactQuery newContact)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element (occurs for each element)
                    var contactName = (from s in context.interfaceinformations
                                       where s.attribute == "contactName"
                                       select s).FirstOrDefault();
                    //Save content (occurs for each element)
                    if (contactName != null)
                        contactName.content = newContact.contactName;

                    var contactPhone = (from s in context.interfaceinformations
                                        where s.attribute == "contactPhone"
                                        select s).FirstOrDefault();
                    if (contactPhone != null)
                        contactPhone.content = newContact.contactPhone;

                    var contactEmail = (from s in context.interfaceinformations
                                        where s.attribute == "contactEmail"
                                        select s).FirstOrDefault();

                    if (contactEmail != null)
                        contactEmail.content = newContact.contactEmail;

                    var contactAdditionalInfo = (from s in context.interfaceinformations
                                                 where s.attribute == "contactAdditionalInfo"
                                                 select s).FirstOrDefault();

                    if (contactAdditionalInfo != null)
                    {
                        newContact.contactAdditionalInfo = newContact.contactAdditionalInfo == null ? "" : newContact.contactAdditionalInfo;
                        contactAdditionalInfo.content = newContact.contactAdditionalInfo;
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveContact error " + ex);
                return false;
            }
        }

        //Send inquire email to CCWiC
        public bool sendContactEmail(ContactEmailQuery info)
        {
            try {
                //Define sender and recipient
                MailAddress sender = new MailAddress(info.contactEmail);
                MailAddress user = new MailAddress(info.contactEmail);
                MailMessage mail = new System.Net.Mail.MailMessage(sender, user);

                //Email Message
                mail.Subject = "CCWiC Inquire";
                mail.Body = "Name: " + info.name + "\r\nEmail: "+info.email+"\r\nMessage: " + info.message;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;

                smtp.Credentials = new NetworkCredential(ccwicEmail, ccwicEmailPass); //OJO
                smtp.EnableSsl = true;

                //Send email
                smtp.Send(mail);

                return true;
            
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.sendContactEmail error " + ex);
                return false;
            }
        }

        //Get content found in the Call for Participation section of the website
        public ParticipationQuery getParticipation()
        {
            try
            {
                ParticipationQuery participation = new ParticipationQuery();
                participation.participationParagraph1 = this.getInterfaceElement("participationParagraph1").content;

                return participation;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getParticipation error " + ex);
                return null;
            }
        }

        //Update content of the Call for Participation section of the website
        public bool saveParticipation(ParticipationQuery newParticipation)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element
                    var participationParagraph1 = (from s in context.interfaceinformations
                                                   where s.attribute == "participationParagraph1"
                                                   select s).FirstOrDefault();
                    //Save content
                    if (participationParagraph1 != null)
                    {
                        newParticipation.participationParagraph1 = newParticipation.participationParagraph1 == null ? "" : newParticipation.participationParagraph1;
                        participationParagraph1.content = newParticipation.participationParagraph1;
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveParticipation error " + ex);
                return false;
            }
        }

        //Get content found in the Registration section of the website
        public RegistrationQuery getRegistrationInfo()
        {
            try
            {
                RegistrationQuery registration = new RegistrationQuery();
                registration.registrationParagraph1 = this.getInterfaceElement("registrationParagraph1").content;
                registration.registrationParagraph2 = this.getInterfaceElement("registrationParagraph2").content;

                //Get registration fees
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var undergraduateStudent = (from usertype in context.usertypes
                                                where usertype.userTypeName == "Undergraduate Student"
                                                select usertype).FirstOrDefault();

                    if (undergraduateStudent != null)
                    {
                        registration.undergraduateStudentFee = (double)undergraduateStudent.registrationCost;
                        registration.undergraduateStudentLateFee = (double)undergraduateStudent.registrationLateFee;
                    }

                    var graduateStudent = (from usertype in context.usertypes
                                           where usertype.userTypeName == "Graduate Student"
                                           select usertype).FirstOrDefault();

                    if (graduateStudent != null)
                    {
                        registration.graduateStudentFee = (double)graduateStudent.registrationCost;
                        registration.graduateStudentLateFee = (double)graduateStudent.registrationLateFee;
                    }


                    var highSchoolStudent = (from usertype in context.usertypes
                                             where usertype.userTypeName == "High School Student"
                                             select usertype).FirstOrDefault();

                    if (highSchoolStudent != null)
                    {
                        registration.highSchoolStudentFee = (double)highSchoolStudent.registrationCost;
                        registration.highSchoolStudentLateFee = (double)highSchoolStudent.registrationLateFee;
                    }

                    var companionStudent = (from usertype in context.usertypes
                                            where usertype.userTypeName == "Companion"
                                            select usertype).FirstOrDefault();

                    if (companionStudent != null)
                    {
                        registration.companionStudentFee = (double)companionStudent.registrationCost;
                        registration.companionStudentLateFee = (double)companionStudent.registrationLateFee;
                    }

                    var professionalAcademia = (from usertype in context.usertypes
                                                where usertype.userTypeName == "Professional Academia"
                                                select usertype).FirstOrDefault();

                    if (professionalAcademia != null)
                    {
                        registration.professionalAcademyFee = (double)professionalAcademia.registrationCost;
                        registration.professionalAcademyLateFee = (double)professionalAcademia.registrationLateFee;
                    }

                    var professionalIndustry = (from usertype in context.usertypes
                                                where usertype.userTypeName == "Professional Industry"
                                                select usertype).FirstOrDefault();
                    if (professionalIndustry != null)
                    {
                        registration.professionalIndustryFee = (double)professionalIndustry.registrationCost;
                        registration.professionalIndustryLateFee = (double)professionalIndustry.registrationLateFee;
                    }
                }

                return registration;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getRegistrationInfo error " + ex);
                return null;
            }
        }

        //Update content found in the Registration section of the website
        public bool saveRegistrationInfo(RegistrationQuery newRegistration)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element (occurs for each element)
                    var registrationParagraph1 = (from s in context.interfaceinformations
                                                  where s.attribute == "registrationParagraph1"
                                                  select s).FirstOrDefault();
                    //Save content (occurs for each element)
                    if (registrationParagraph1 != null)
                    {
                        newRegistration.registrationParagraph1 = newRegistration.registrationParagraph1 == null ? "" : newRegistration.registrationParagraph1;
                        registrationParagraph1.content = newRegistration.registrationParagraph1;
                    }

                    var registrationParagraph2 = (from s in context.interfaceinformations
                                                  where s.attribute == "registrationParagraph2"
                                                  select s).FirstOrDefault();

                    if (registrationParagraph2 != null)
                    {
                        newRegistration.registrationParagraph2 = newRegistration.registrationParagraph2 == null ? "" : newRegistration.registrationParagraph2;
                        registrationParagraph2.content = newRegistration.registrationParagraph2;
                    }


                    var undergraduateStudentFee = (from s in context.usertypes
                                                   where s.userTypeName == "Undergraduate Student"
                                                   select s).FirstOrDefault();

                    if (undergraduateStudentFee != null)
                    {
                        undergraduateStudentFee.registrationCost = newRegistration.undergraduateStudentFee;
                        undergraduateStudentFee.registrationLateFee = newRegistration.undergraduateStudentLateFee;
                    }

                    var graduateStudentFee = (from s in context.usertypes
                                              where s.userTypeName == "Graduate Student"
                                              select s).FirstOrDefault();

                    if (graduateStudentFee != null)
                    {
                        graduateStudentFee.registrationCost = newRegistration.graduateStudentFee;
                        graduateStudentFee.registrationLateFee = newRegistration.graduateStudentLateFee;
                    }

                    var highSchoolStudentFee = (from s in context.usertypes
                                                where s.userTypeName == "High School Student"
                                                select s).FirstOrDefault();

                    if (highSchoolStudentFee != null)
                    {
                        highSchoolStudentFee.registrationCost = newRegistration.highSchoolStudentFee;
                        highSchoolStudentFee.registrationLateFee = newRegistration.highSchoolStudentLateFee;
                    }

                    var companionStudentFee = (from s in context.usertypes
                                               where s.userTypeName == "Companion"
                                               select s).FirstOrDefault();

                    if (companionStudentFee != null)
                    {
                        companionStudentFee.registrationCost = newRegistration.companionStudentFee;
                        companionStudentFee.registrationLateFee = newRegistration.companionStudentLateFee;
                    }

                    var professionalIndustryFee = (from s in context.usertypes
                                                   where s.userTypeName == "Professional Industry"
                                                   select s).FirstOrDefault();

                    if (professionalIndustryFee != null)
                    {
                        professionalIndustryFee.registrationCost = newRegistration.professionalIndustryFee;
                        professionalIndustryFee.registrationLateFee = newRegistration.professionalIndustryLateFee;
                    }

                    var professionalAcademyFee = (from s in context.usertypes
                                                  where s.userTypeName == "Professional Academia"
                                                  select s).FirstOrDefault();

                    if (professionalAcademyFee != null)
                    {
                        professionalAcademyFee.registrationCost = newRegistration.professionalAcademyFee;
                        professionalAcademyFee.registrationLateFee = newRegistration.professionalAcademyLateFee;
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveRegistrationInfo error " + ex);
                return false;
            }
        }

        //Get content found in the Administator->Deadlines section of the website
        public DeadlinesQuery getDeadlines()
        {
            try
            {
                //Get Registration deadlines, and custom deadlines
                DeadlinesQuery deadline = new DeadlinesQuery();
                deadline.deadline1 = this.getInterfaceElement("deadline1").content;
                deadline.deadlineDate1 = this.getInterfaceElement("deadlineDate1").content;
                deadline.deadline2 = this.getInterfaceElement("deadline2").content;
                deadline.deadlineDate2 = this.getInterfaceElement("deadlineDate2").content;
                deadline.deadline3 = this.getInterfaceElement("deadline3").content;
                deadline.deadlineDate3 = this.getInterfaceElement("deadlineDate3").content;
                deadline.registrationDeadline = this.getInterfaceElement("registrationDeadline").content;
                deadline.lateRegistrationDeadline = this.getInterfaceElement("lateRegistrationDeadline").content;
                deadline.paragraph = this.getInterfaceElement("deadlineParagraph1").content;

                //Submission Deadlines
                deadline.extendedPaperDeadline = this.getDeadlineElement("Extended Paper").content;
                deadline.posterDeadline = this.getDeadlineElement("Poster ").content;
                deadline.panelDeadline = this.getDeadlineElement("Panel").content;
                deadline.othersDeadline = this.getDeadlineElement("Others").content;
                deadline.workshopDeadline = this.getDeadlineElement("Workshop").content;
                deadline.sponsorDeadline = this.getInterfaceElement("sponsorDeadline").content;

                return deadline;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getDeadline error " + ex);
                return null;
            }
        }

        //Convert mm/dd/yyyy date to 'Day of the Week, Day of the Month, Month, Year' string format
        public String convertDates(String deadline)
        {
            String result = "";

            if (deadline != null && deadline != "")
            {
                string[] date = deadline.Split('/');
                if (date.Count() == 3)
                {
                    DateTime d = new DateTime( // Constructor (Year, Month, Day)
                        Convert.ToInt32(date[2]),
                        Convert.ToInt32(date[0]),
                        Convert.ToInt32(date[1]));
                    //Split date and convert to string
                    result = d.DayOfWeek + ", " + d.ToString("MMMM", CultureInfo.InvariantCulture) + " " + d.Day + ", " + d.Year;
                }
            }

            return result;
        }

        //Get content found in the Deadlines section of the website
        public DeadlinesQuery getInterfaceDeadlines()
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    DeadlinesQuery deadline = new DeadlinesQuery();
                    deadline.deadline1 = this.getInterfaceElement("deadline1").content;
                    deadline.deadlineDate1 = this.getInterfaceElement("deadlineDate1").content;
                    deadline.deadline2 = this.getInterfaceElement("deadline2").content;
                    deadline.deadlineDate2 = this.getInterfaceElement("deadlineDate2").content;
                    deadline.deadline3 = this.getInterfaceElement("deadline3").content;
                    deadline.deadlineDate3 = this.getInterfaceElement("deadlineDate3").content;
                    deadline.registrationDeadline = this.getInterfaceElement("registrationDeadline").content;
                    deadline.lateRegistrationDeadline = this.getInterfaceElement("lateRegistrationDeadline").content;
                    deadline.paragraph = this.getInterfaceElement("deadlineParagraph1").content;

                    deadline.extendedPaperDeadline = this.getDeadlineElement("Extended Paper").content;
                    deadline.posterDeadline = this.getDeadlineElement("Poster ").content;
                    deadline.panelDeadline = this.getDeadlineElement("Panel").content;
                    deadline.othersDeadline = this.getDeadlineElement("Others").content;
                    deadline.workshopDeadline = this.getDeadlineElement("Workshop").content;
                    deadline.sponsorDeadline = this.getInterfaceElement("sponsorDeadline").content;

                    //Convert Dates
                    deadline.deadlineDate1 = this.convertDates(deadline.deadlineDate1);
                    deadline.deadlineDate2 = this.convertDates(deadline.deadlineDate2);
                    deadline.deadlineDate3 = this.convertDates(deadline.deadlineDate3);
                    deadline.registrationDeadline = this.convertDates(deadline.registrationDeadline);
                    deadline.lateRegistrationDeadline = this.convertDates(deadline.lateRegistrationDeadline);
                    deadline.extendedPaperDeadline = this.convertDates(deadline.extendedPaperDeadline);
                    deadline.posterDeadline = this.convertDates(deadline.posterDeadline);
                    deadline.panelDeadline = this.convertDates(deadline.panelDeadline);
                    deadline.othersDeadline = this.convertDates(deadline.othersDeadline);
                    deadline.workshopDeadline = this.convertDates(deadline.workshopDeadline);
                    deadline.sponsorDeadline = this.convertDates(deadline.sponsorDeadline);
                                       
                    return deadline;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getInterfaceDeadlines error " + ex);
                return null;
            }
        }

        //Update content found in the Deadlines section of the website
        public bool saveDeadlines(DeadlinesQuery newDeadline)
        {
            try
            {
                //Check the dates are not invalid.
                if (newDeadline.deadlineDate1 == "Invalid Date")
                {
                    newDeadline.deadlineDate1 = "";
                }

                if (newDeadline.deadlineDate2 == "Invalid Date")
                {
                    newDeadline.deadlineDate2 = "";
                }

                if (newDeadline.deadlineDate3 == "Invalid Date")
                {
                    newDeadline.deadlineDate3 = "";
                }

                if (newDeadline.registrationDeadline == "Invalid Date")
                {
                    newDeadline.registrationDeadline = "";
                }

                if (newDeadline.lateRegistrationDeadline == "Invalid Date")
                {
                    newDeadline.lateRegistrationDeadline = "";
                }

                //Check submission dates are not invalid
                if (newDeadline.extendedPaperDeadline == "Invalid Date")
                {
                    newDeadline.extendedPaperDeadline = "";
                }

                if (newDeadline.posterDeadline == "Invalid Date")
                {
                    newDeadline.posterDeadline = "";
                }

                if (newDeadline.panelDeadline == "Invalid Date")
                {
                    newDeadline.panelDeadline = "";
                }

                if (newDeadline.othersDeadline == "Invalid Date")
                {
                    newDeadline.othersDeadline = "";
                }

                if (newDeadline.workshopDeadline == "Invalid Date")
                {
                    newDeadline.workshopDeadline = "";
                }

                if (newDeadline.sponsorDeadline == "Invalid Date")
                {
                    newDeadline.sponsorDeadline = "";
                }

                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element (occurs for each element)
                    var paragraph = (from s in context.interfaceinformations
                                     where s.attribute == "deadlineParagraph1"
                                     select s).FirstOrDefault();
                    //Save content (occurs for each element)
                    if (paragraph != null)
                    {
                        newDeadline.paragraph = newDeadline.paragraph == null ? "" : newDeadline.paragraph;
                        paragraph.content = newDeadline.paragraph;
                    }

                    var deadline1 = (from s in context.interfaceinformations
                                     where s.attribute == "deadline1"
                                     select s).FirstOrDefault();
                    if (deadline1 != null)
                        deadline1.content = newDeadline.deadline1;

                    var deadline2 = (from s in context.interfaceinformations
                                     where s.attribute == "deadline2"
                                     select s).FirstOrDefault();
                    if (deadline2 != null)
                        deadline2.content = newDeadline.deadline2;

                    var deadline3 = (from s in context.interfaceinformations
                                     where s.attribute == "deadline3"
                                     select s).FirstOrDefault();

                    if (deadline3 != null)
                        deadline3.content = newDeadline.deadline3;

                    var deadlineDate1 = (from s in context.interfaceinformations
                                         where s.attribute == "deadlineDate1"
                                         select s).FirstOrDefault();

                    if (deadlineDate1 != null)
                        deadlineDate1.content = newDeadline.deadlineDate1;

                    var deadlineDate2 = (from s in context.interfaceinformations
                                         where s.attribute == "deadlineDate2"
                                         select s).FirstOrDefault();

                    if (deadlineDate2 != null)
                        deadlineDate2.content = newDeadline.deadlineDate2;

                    var deadlineDate3 = (from s in context.interfaceinformations
                                         where s.attribute == "deadlineDate3"
                                         select s).FirstOrDefault();

                    if (deadlineDate3 != null)
                        deadlineDate3.content = newDeadline.deadlineDate3;

                    var registrationDeadline = (from s in context.interfaceinformations
                                                where s.attribute == "registrationDeadline"
                                         select s).FirstOrDefault();

                    if (registrationDeadline != null)
                        registrationDeadline.content = newDeadline.registrationDeadline;

                    var lateRegistrationDeadline = (from s in context.interfaceinformations
                                                    where s.attribute == "lateRegistrationDeadline"
                                                    select s).FirstOrDefault();

                    if (lateRegistrationDeadline != null)
                        lateRegistrationDeadline.content = newDeadline.lateRegistrationDeadline;

                    var sponsorDeadline = (from s in context.interfaceinformations
                                              where s.attribute == "sponsorDeadline"
                                              select s).FirstOrDefault();

                    if (sponsorDeadline != null)
                        sponsorDeadline.content = newDeadline.sponsorDeadline;

                    //Submission deadlines
                    var extendedPaperDeadline = (from s in context.submissiontypes
                                              where s.name == "Extended Paper"
                                              select s).FirstOrDefault();

                    if (extendedPaperDeadline != null)
                        extendedPaperDeadline.deadline = newDeadline.extendedPaperDeadline;

                    var posterDeadline = (from s in context.submissiontypes
                                          where s.name == "Poster"
                                          select s).FirstOrDefault();

                    if (posterDeadline != null)
                        posterDeadline.deadline = newDeadline.posterDeadline;

                    var panelDeadline = (from s in context.submissiontypes
                                         where s.name == "Panel"
                                         select s).FirstOrDefault();

                    if (panelDeadline != null)
                        panelDeadline.deadline = newDeadline.panelDeadline;

                    var othersDeadline = (from s in context.submissiontypes
                                       where s.name == "Others"
                                              select s).FirstOrDefault();

                    if (othersDeadline != null)
                        othersDeadline.deadline = newDeadline.othersDeadline;

                    var workshopDeadline = (from s in context.submissiontypes
                                            where s.name == "Workshop"
                                            select s).FirstOrDefault();

                    if (workshopDeadline != null)
                        workshopDeadline.deadline = newDeadline.workshopDeadline;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveDeadlines error " + ex);
                return false;
            }
        }

        //Get content found in the Committee section of the website
        public CommitteeQuery getCommittee()
        {
            try
            {
                var committee = new CommitteeQuery();

                committee.committee = this.getInterfaceElement("committee").content;

                return committee;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getCommittee error " + ex);
                return null;
            }
        }

        //Update content found in the Committee section of the website
        public bool saveCommittee(CommitteeQuery info)
        {
            String committee= info.committee;

            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element
                    var i = (from s in context.interfaceinformations
                             where s.attribute == "committee"
                             select s).FirstOrDefault();

                    //Save content
                    if (i != null)
                    {
                        committee = committee == null ? "" : committee;
                        i.content = committee;
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveCommittee error " + ex);
                return false;
            }
        }

        //Get benefits under the specfied sponsor category
        public SponsorInterfaceBenefits getAdminSponsorBenefits(String name)
        {
            try
            {
                var sponsors = new SponsorInterfaceBenefits();

                using (conferenceadminContext context = new conferenceadminContext())
                {
                    if (name == "Diamond")
                    {
                        var diamond = (from s in context.sponsortypes
                                        where s.name == "Diamond"
                                        select s).FirstOrDefault();
                        if (diamond != null)
                        {
                            sponsors.diamondAmount = diamond.amount;
                            SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                            benefits.benefit1 = diamond.benefit1;
                            benefits.benefit2 = diamond.benefit2;
                            benefits.benefit3 = diamond.benefit3;
                            benefits.benefit4 = diamond.benefit4;
                            benefits.benefit5 = diamond.benefit5;
                            benefits.benefit6 = diamond.benefit6;
                            benefits.benefit7 = diamond.benefit7;
                            benefits.benefit8 = diamond.benefit8;
                            benefits.benefit9 = diamond.benefit9;
                            benefits.benefit10 = diamond.benefit10;
                            sponsors.diamondBenefits = benefits;

                        }
                    }

                    else if (name == "Platinum")
                    {
                        var platinum = (from s in context.sponsortypes
                                        where s.name == "Platinum"
                                        select s).FirstOrDefault();
                        if (platinum != null)
                        {
                            sponsors.platinumAmount = platinum.amount;
                            SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                            benefits.benefit1 = platinum.benefit1;
                            benefits.benefit2 = platinum.benefit2;
                            benefits.benefit3 = platinum.benefit3;
                            benefits.benefit4 = platinum.benefit4;
                            benefits.benefit5 = platinum.benefit5;
                            benefits.benefit6 = platinum.benefit6;
                            benefits.benefit7 = platinum.benefit7;
                            benefits.benefit8 = platinum.benefit8;
                            benefits.benefit9 = platinum.benefit9;
                            benefits.benefit10 = platinum.benefit10;
                            sponsors.platinumBenefits = benefits;

                        }
                    }

                    else if (name == "Gold")
                    {
                        var gold = (from s in context.sponsortypes
                                    where s.name == "Gold"
                                    select s).FirstOrDefault();
                        if (gold != null)
                        {
                            sponsors.goldAmount = gold.amount;
                            SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                            benefits.benefit1 = gold.benefit1;
                            benefits.benefit2 = gold.benefit2;
                            benefits.benefit3 = gold.benefit3;
                            benefits.benefit4 = gold.benefit4;
                            benefits.benefit5 = gold.benefit5;
                            benefits.benefit6 = gold.benefit6;
                            benefits.benefit7 = gold.benefit7;
                            benefits.benefit8 = gold.benefit8;
                            benefits.benefit9 = gold.benefit9;
                            benefits.benefit10 = gold.benefit10;
                            sponsors.goldBenefits = benefits;

                        }
                    }

                    else if (name == "Silver")
                    {
                        var silver = (from s in context.sponsortypes
                                      where s.name == "Silver"
                                      select s).FirstOrDefault();
                        if (silver != null)
                        {
                            sponsors.silverAmount = silver.amount;
                            SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                            benefits.benefit1 = silver.benefit1;
                            benefits.benefit2 = silver.benefit2;
                            benefits.benefit3 = silver.benefit3;
                            benefits.benefit4 = silver.benefit4;
                            benefits.benefit5 = silver.benefit5;
                            benefits.benefit6 = silver.benefit6;
                            benefits.benefit7 = silver.benefit7;
                            benefits.benefit8 = silver.benefit8;
                            benefits.benefit9 = silver.benefit9;
                            benefits.benefit10 = silver.benefit10;
                            sponsors.silverBenefits = benefits;
                        }
                    }

                    else if (name == "Bronze")
                    {
                        var bronze = (from s in context.sponsortypes
                                      where s.name == "Bronze"
                                      select s).FirstOrDefault();
                        if (bronze != null)
                        {
                            sponsors.bronzeAmount = bronze.amount;
                            SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                            benefits.benefit1 = bronze.benefit1;
                            benefits.benefit2 = bronze.benefit2;
                            benefits.benefit3 = bronze.benefit3;
                            benefits.benefit4 = bronze.benefit4;
                            benefits.benefit5 = bronze.benefit5;
                            benefits.benefit6 = bronze.benefit6;
                            benefits.benefit7 = bronze.benefit7;
                            benefits.benefit8 = bronze.benefit8;
                            benefits.benefit9 = bronze.benefit9;
                            benefits.benefit10 = bronze.benefit10;
                            sponsors.bronzeBenefits = benefits;
                        }
                    }
                }

                return sponsors;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getAdminSponsorBenefits error " + ex);
                return null;
            }
        }

        //Update benefits under a sponsor cateogry
        public bool saveSponsorBenefits(SaveSponsorQuery sponsorBenefits)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var sponsor = new sponsortype();
                    //Get element
                    if (sponsorBenefits.name == "Diamond")
                    {
                        sponsor = (from s in context.sponsortypes
                                   where s.name == "Diamond"
                                   select s).FirstOrDefault();
                    }

                    else if (sponsorBenefits.name == "Platinum")
                    {
                        sponsor = (from s in context.sponsortypes
                                   where s.name == "Platinum"
                                   select s).FirstOrDefault();
                    }
                    else if (sponsorBenefits.name == "Gold")
                    {
                        sponsor = (from s in context.sponsortypes
                                   where s.name == "Gold"
                                   select s).FirstOrDefault();
                    }
                    else if (sponsorBenefits.name == "Silver")
                    {
                        sponsor = (from s in context.sponsortypes
                                   where s.name == "Silver"
                                   select s).FirstOrDefault();
                    }

                    else if (sponsorBenefits.name == "Bronze")
                    {
                        sponsor = (from s in context.sponsortypes
                                   where s.name == "Bronze"
                                   select s).FirstOrDefault();
                    }
                    //Save content
                    if (sponsor != null)
                    {
                        sponsor.amount = sponsorBenefits.amount;
                        sponsor.benefit1 = sponsorBenefits.benefits.benefit1;
                        sponsor.benefit2 = sponsorBenefits.benefits.benefit2;
                        sponsor.benefit3 = sponsorBenefits.benefits.benefit3;
                        sponsor.benefit4 = sponsorBenefits.benefits.benefit4;
                        sponsor.benefit5 = sponsorBenefits.benefits.benefit5;
                        sponsor.benefit6 = sponsorBenefits.benefits.benefit6;
                        sponsor.benefit7 = sponsorBenefits.benefits.benefit7;
                        sponsor.benefit8 = sponsorBenefits.benefits.benefit8;
                        sponsor.benefit9 = sponsorBenefits.benefits.benefit9;
                        sponsor.benefit10 = sponsorBenefits.benefits.benefit10;
                        context.SaveChanges();
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveSponsorBenefits error " + ex);
                return false;
            }
        }

        //Update text found in the Sponsor section of the website
        public bool saveInstructions(SponsorInterfaceBenefits info)
        {
            String instructions = info.instructions;

            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element
                    var i = (from s in context.interfaceinformations
                             where s.attribute == "sponsorInstructions"
                             select s).FirstOrDefault();
                    //Save content
                    if (i != null)
                    {
                        instructions = instructions == null ? "" : instructions;
                        i.content = instructions;
                    }

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveInstructions error " + ex);
                return false;
            }
        }

        //Get text found in the Sponsor section of the website
        public String getInstructions()
        {
            try
            {
                var instructions = this.getInterfaceElement("sponsorInstructions").content;
                return instructions;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getInstructions error " + ex);
                return null;
            }
        }

        //Get benefits of all sponsors for Sponsors section of the website (web content)
        public SponsorInterfaceBenefits getAllSponsorBenefits()
        {
            try
            {
                var sponsors = new SponsorInterfaceBenefits();

                using (conferenceadminContext context = new conferenceadminContext())
                {
                    var diamond = (from s in context.sponsortypes
                                    where s.name == "Diamond"
                                    select s).FirstOrDefault();
                    if (diamond != null)
                    {
                        sponsors.diamondAmount = diamond.amount;
                        SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                        benefits.benefit1 = diamond.benefit1;
                        benefits.benefit2 = diamond.benefit2;
                        benefits.benefit3 = diamond.benefit3;
                        benefits.benefit4 = diamond.benefit4;
                        benefits.benefit5 = diamond.benefit5;
                        benefits.benefit6 = diamond.benefit6;
                        benefits.benefit7 = diamond.benefit7;
                        benefits.benefit8 = diamond.benefit8;
                        benefits.benefit9 = diamond.benefit9;
                        benefits.benefit10 = diamond.benefit10;
                        sponsors.diamondBenefits = benefits;
                    }

                    var platinum = (from s in context.sponsortypes
                                    where s.name == "Platinum"
                                    select s).FirstOrDefault();
                    if (platinum != null)
                    {
                        sponsors.platinumAmount = platinum.amount;
                        SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                        benefits.benefit1 = platinum.benefit1;
                        benefits.benefit2 = platinum.benefit2;
                        benefits.benefit3 = platinum.benefit3;
                        benefits.benefit4 = platinum.benefit4;
                        benefits.benefit5 = platinum.benefit5;
                        benefits.benefit6 = platinum.benefit6;
                        benefits.benefit7 = platinum.benefit7;
                        benefits.benefit8 = platinum.benefit8;
                        benefits.benefit9 = platinum.benefit9;
                        benefits.benefit10 = platinum.benefit10;
                        sponsors.platinumBenefits = benefits;

                    }

                    var gold = (from s in context.sponsortypes
                                where s.name == "Gold"
                                select s).FirstOrDefault();
                    if (gold != null)
                    {
                        sponsors.goldAmount = gold.amount;
                        SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                        benefits.benefit1 = gold.benefit1;
                        benefits.benefit2 = gold.benefit2;
                        benefits.benefit3 = gold.benefit3;
                        benefits.benefit4 = gold.benefit4;
                        benefits.benefit5 = gold.benefit5;
                        benefits.benefit6 = gold.benefit6;
                        benefits.benefit7 = gold.benefit7;
                        benefits.benefit8 = gold.benefit8;
                        benefits.benefit9 = gold.benefit9;
                        benefits.benefit10 = gold.benefit10;
                        sponsors.goldBenefits = benefits;

                    }

                    var silver = (from s in context.sponsortypes
                                  where s.name == "Silver"
                                  select s).FirstOrDefault();
                    if (silver != null)
                    {
                        sponsors.silverAmount = silver.amount;
                        SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                        benefits.benefit1 = silver.benefit1;
                        benefits.benefit2 = silver.benefit2;
                        benefits.benefit3 = silver.benefit3;
                        benefits.benefit4 = silver.benefit4;
                        benefits.benefit5 = silver.benefit5;
                        benefits.benefit6 = silver.benefit6;
                        benefits.benefit7 = silver.benefit7;
                        benefits.benefit8 = silver.benefit8;
                        benefits.benefit9 = silver.benefit9;
                        benefits.benefit10 = silver.benefit10;
                        sponsors.silverBenefits = benefits;
                    }

                    var bronze = (from s in context.sponsortypes
                                  where s.name == "Bronze"
                                  select s).FirstOrDefault();
                    if (bronze != null)
                    {
                        sponsors.bronzeAmount = bronze.amount;
                        SponsorBenefitsQuery benefits = new SponsorBenefitsQuery();
                        benefits.benefit1 = bronze.benefit1;
                        benefits.benefit2 = bronze.benefit2;
                        benefits.benefit3 = bronze.benefit3;
                        benefits.benefit4 = bronze.benefit4;
                        benefits.benefit5 = bronze.benefit5;
                        benefits.benefit6 = bronze.benefit6;
                        benefits.benefit7 = bronze.benefit7;
                        benefits.benefit8 = bronze.benefit8;
                        benefits.benefit9 = bronze.benefit9;
                        benefits.benefit10 = bronze.benefit10;
                        sponsors.bronzeBenefits = benefits;
                    }
                }

                return sponsors;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getAllSponsorBenefits error " + ex);
                return null;
            }
        }

        //Get general content found in the conference
        public GeneralInfoQuery getGeneralInfo()
        {
            try
            {
                GeneralInfoQuery info = new GeneralInfoQuery();
                info.conferenceAcronym = this.getInterfaceElement("conferenceAcronym").content;
                info.conferenceName = this.getInterfaceElement("conferenceName").content;
                info.dateFrom = this.getInterfaceElement("conferenceDay1").content;
                info.dateTo = this.getInterfaceElement("conferenceDay3").content;

                if (info.dateTo == "")
                {
                    info.dateTo = this.getInterfaceElement("conferenceDay2").content;
                }

                return info;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getGeneralInfo error " + ex);
                return null;
            }
        }

        //Get conference logo
        public GeneralInfoQuery getWebsiteLogo()
        {
            try
            {
                GeneralInfoQuery logo = new GeneralInfoQuery();
                using (conferenceadminContext context = new conferenceadminContext())
                {

                    var img = (from s in context.interfacedocuments
                               where s.attibuteName == "logo"
                               select s).FirstOrDefault();

                    if (img != null)
                    {
                        logo.logo = img.content;
                    }
                }

                return logo;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getWebsiteLogo error " + ex);
                return null;
            }
        }

        //Update general content found in the website
        public bool saveGeneralInfo(GeneralInfoQuery info)
        {
            try
            {
                //Check dates are valid
                String conferenceDay1 = "";
                String conferenceDay2 = "";
                String conferenceDay3 = "";

                if (info.dateFrom == "" && info.dateTo != "")
                {
                    return false;
                }

                else if (info.dateFrom != "" && info.dateTo == "")
                {
                    conferenceDay1 = info.dateFrom;
                }

                else if (info.dateFrom != "" && info.dateTo != "" && info.dateFrom.Split('/').Count() > 2 && info.dateTo.Split('/').Count() > 2)
                {                  
                    //check distance between dates

                    //string from = Convert.ToDateTime(info.dateFrom).ToShortDateString();
                    //string to = Convert.ToDateTime(info.dateTo).ToShortDateString();

                    var fromDay = Convert.ToInt32(info.dateFrom.Split('/')[1]);
                    var fromMonth = Convert.ToInt32(info.dateFrom.Split('/')[0]);
                    var fromYear = Convert.ToInt32(info.dateFrom.Split('/')[2]);

                    var toDay = Convert.ToInt32(info.dateTo.Split('/')[1]);
                    var toMonth = Convert.ToInt32(info.dateTo.Split('/')[0]);
                    var toYear = Convert.ToInt32(info.dateTo.Split('/')[2]);

                    // Constructor (Year, Month, Day)

                    DateTime dateFrom = new DateTime(fromYear, fromMonth, fromDay);
                    DateTime dateTo = new DateTime(toYear, toMonth, toDay);

                    double differenceDays = dateTo.Subtract(dateFrom).TotalDays;

                    //Depending on the difference assign 1, 2 or 3 conference days
                    if (differenceDays < 0 || differenceDays >= 3)
                    {
                        return false;
                    }

                    else if (differenceDays == 0)
                    {
                        conferenceDay1 = info.dateFrom;
                    }

                    else if (differenceDays == 1)
                    {
                        conferenceDay1 = info.dateFrom;
                        conferenceDay2 = info.dateTo;
                    }
                    else if (differenceDays == 2)
                    {
                        conferenceDay1 = info.dateFrom;
                        DateTime d2 = dateFrom.AddDays(1);
                        var month = d2.Month; var day = d2.Day; var year = d2.Year;
                        conferenceDay2 = month + "/" + day + "/" + year;
                        conferenceDay3 = info.dateTo;
                    }
                }

                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element (occurs for each element)
                    var conferenceAcronym = (from s in context.interfaceinformations
                                             where s.attribute == "conferenceAcronym"
                                             select s).FirstOrDefault();
                    //Save element (occurs for each element)
                    if (conferenceAcronym != null)
                        conferenceAcronym.content = info.conferenceAcronym;

                    var conferenceName = (from s in context.interfaceinformations
                                          where s.attribute == "conferenceName"
                                          select s).FirstOrDefault();
                    if (conferenceName != null)
                        conferenceName.content = info.conferenceName;

                    var d1 = (from s in context.interfaceinformations
                              where s.attribute == "conferenceDay1"
                              select s).FirstOrDefault();

                    if (d1 != null)
                        d1.content = conferenceDay1;

                    var d2 = (from s in context.interfaceinformations
                              where s.attribute == "conferenceDay2"
                              select s).FirstOrDefault();

                    if (d2 != null)
                        d2.content = conferenceDay2;

                    var d3 = (from s in context.interfaceinformations
                              where s.attribute == "conferenceDay3"
                              select s).FirstOrDefault();

                    if (d3 != null)
                        d3.content = conferenceDay3;

                    var logo = (from s in context.interfacedocuments
                                where s.attibuteName == "logo"
                                select s).FirstOrDefault();

                    if (logo != null && info.logo != null && info.logo != "")
                        logo.content = info.logo;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveGeneralInfo error " + ex);
                return false;
            }
        }

        //Get conference program documents: Program Schedule and Abstracts
        public ProgramQuery getProgram()
        {
            try
            {
                ProgramQuery programInfo = new ProgramQuery();

                using (conferenceadminContext context = new conferenceadminContext())
                {

                    var program = (from s in context.interfacedocuments
                                   where s.attibuteName == "agendaPDF"
                                   select s).FirstOrDefault();

                    if (program != null)
                    {
                        programInfo.program = program.content;
                        programInfo.pattribute = "agendaPDF";
                    }

                    var abstracts = (from s in context.interfacedocuments
                                     where s.attibuteName == "abstractPdf"
                                     select s).FirstOrDefault();

                    if (abstracts != null)
                    {
                        programInfo.abstracts = abstracts.content;
                        programInfo.aattribute = "abstractPdf";
                    }
                }

                return programInfo;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getProgram error " + ex);
                return null;
            }
        }

        public ProgramQuery getProgramDocument()
        {

            try
            {
                ProgramQuery programInfo = new ProgramQuery();

                using (conferenceadminContext context = new conferenceadminContext())
                {

                    var program = (from s in context.interfacedocuments
                                   where s.attibuteName == "agendaPDF"
                                   select s).FirstOrDefault();

                    if (program != null)
                    {
                        programInfo.program = program.content;
                    }
                }

                return programInfo;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getProgramDocument error " + ex);
                return null;
            }
        }

        public ProgramQuery getAbstractDocument()
        {
            try
            {
                ProgramQuery programInfo = new ProgramQuery();

                using (conferenceadminContext context = new conferenceadminContext())
                {

                    var abstracts = (from s in context.interfacedocuments
                                     where s.attibuteName == "abstractPdf"
                                     select s).FirstOrDefault();

                    if (abstracts != null)
                    {
                        programInfo.abstracts = abstracts.content;
                    }
                }

                return programInfo;
            }
            catch (Exception ex)
            {
                Console.Write("WebManager.getAbstractDocument error " + ex);
                return null;
            }
        }

        //Update conference program documents
        public bool saveProgram(ProgramQuery programInfo)
        {
            try
            {
                using (conferenceadminContext context = new conferenceadminContext())
                {
                    //Get element (occurs for each element)
                    var program = (from s in context.interfacedocuments
                                   where s.attibuteName == "agendaPDF"
                                   select s).FirstOrDefault();
                    //Save content
                    if (program != null && programInfo.program != null && programInfo.program != "")
                        program.content = programInfo.program;

                    var abstracts = (from s in context.interfacedocuments
                                     where s.attibuteName == "abstractPdf"
                                     select s).FirstOrDefault();

                    if (abstracts != null && programInfo.abstracts != null && programInfo.abstracts != "")
                        abstracts.content = programInfo.abstracts;

                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.Write("WebManger.saveProgram error " + ex);
                return false;
            }
        }

    }

    public class ContentQuery
    {
        public int interfaceID;
        public String content;

        public ContentQuery()
        {

        }
    }

    public class HomeQuery
    {
        public String conferenceName;
        public String homeMainTitle;
        public String homeParagraph1;
        public String image;

        public HomeQuery()
        {

        }
    }

    public class GeneralInfoQuery
    {
        public String conferenceAcronym;
        public String conferenceName;
        public String dateFrom;
        public String dateTo;
        public String logo;
        public GeneralInfoQuery()
        {

        }
    }

    public class VenueQuery
    {
        public String venueParagraph1;
        public String venueTitleBox;
        public String venueParagraphContentBox;

        public VenueQuery()
        {

        }
    }

    public class ContactQuery
    {
        public String contactName;
        public String contactPhone;
        public String contactEmail;
        public String contactAdditionalInfo;

        public ContactQuery()
        {

        }
    }

    public class ParticipationQuery
    {
        public String participationParagraph1;

        public ParticipationQuery()
        {

        }
    }

    public class RegistrationQuery
    {
        public String registrationParagraph1;
        public String registrationParagraph2;
        public double undergraduateStudentFee;
        public double graduateStudentFee;
        public double highSchoolStudentFee;
        public double companionStudentFee;
        public double professionalAcademyFee;
        public double professionalIndustryFee;
        public double undergraduateStudentLateFee;
        public double graduateStudentLateFee;
        public double highSchoolStudentLateFee;
        public double companionStudentLateFee;
        public double professionalAcademyLateFee;
        public double professionalIndustryLateFee;

        public RegistrationQuery()
        {

        }
    }

    public class DeadlinesQuery
    {
        public String deadline1;
        public String deadlineDate1;
        public String deadline2;
        public String deadlineDate2;
        public String deadline3;
        public String deadlineDate3;
        public String registrationDeadline;
        public String lateRegistrationDeadline;
        public String extendedPaperDeadline;
        public String posterDeadline;
        public String panelDeadline;
        public String othersDeadline;
        public String workshopDeadline;
        public String sponsorDeadline;
        public String paragraph;
    }

    public class CommitteeQuery
    {
        public String committee;
        public CommitteeQuery()
        {

        }
    }

    public class SponsorBenefitsQuery
    {
        public String benefit1;
        public String benefit2;
        public String benefit3;
        public String benefit4;
        public String benefit5;
        public String benefit6;
        public String benefit7;
        public String benefit8;
        public String benefit9;
        public String benefit10;

        public SponsorBenefitsQuery()
        {

        }
    }

    public class SponsorInterfaceBenefits
    {
        public String instructions;
        public double diamondAmount;
        public double platinumAmount;
        public double goldAmount;
        public double silverAmount;
        public double bronzeAmount;
        public SponsorBenefitsQuery diamondBenefits;
        public SponsorBenefitsQuery platinumBenefits;
        public SponsorBenefitsQuery goldBenefits;
        public SponsorBenefitsQuery silverBenefits;
        public SponsorBenefitsQuery bronzeBenefits;

        public SponsorInterfaceBenefits()
        {

        }
    }

    public class SaveSponsorQuery
    {
        public String name;
        public double amount;
        public SponsorBenefitsQuery benefits;

        public SaveSponsorQuery()
        {

        }
    }

    public class ProgramQuery
    {
        public String program;
        public String pattribute;
        public String abstracts;
        public String aattribute;

        public ProgramQuery()
        {

        }
    }

    public class ContactEmailQuery
    {
        public String name;
        public String email;
        public String contactEmail;
        public String message;
    }
}