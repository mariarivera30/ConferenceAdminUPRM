﻿using Nancy;
using Nancy.Responses;
using NancyService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.ModelBinding;
using Nancy.Authentication.Token;
using Nancy.Security;

namespace NancyService.Modules
{
    public class AdminModules : NancyModule
    {
        public AdminModules(ITokenizer tokenizer)
            : base("/admin")
        {
            WebManager webManager = new WebManager();
            ReportManager reportManager = new ReportManager();
            AdminManager adminManager = new AdminManager();
            EvaluatorManager evaluatorManager = new EvaluatorManager();
            TopicManager topicManager = new TopicManager();
            SponsorManager sponsorManager = new SponsorManager();
            RegistrationManager registration = new RegistrationManager();
            GuestManager guest = new GuestManager();
            TemplateManager templateManager = new TemplateManager();
            AuthTemplateManager authTemplateManager = new AuthTemplateManager();
            SubmissionManager submissionManager = new SubmissionManager();
            BannerManager bannerManager = new BannerManager();

            /*------------------Payment--------------------------*/
            Post["/secureReentry"] = parameters =>
            {
                /*receive the tandem ID  and information store on data base and confirm payment
                /*return in the xml the the receipt link or the error link*/
                return Response.AsXml("");

            };

            /* ----- Template -----*/


            Post["/addTemplate"] = parameters =>
            {
                var temp = this.Bind<TemplateManager.templateQuery>();
                TemplateManager.templateQuery result = templateManager.addTemplate(temp);
                if (result != null)
                {
                    return Response.AsJson(result);
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }

            };

            Get["/getTemplatesAdmin"] = parameters =>
            {

                return Response.AsJson(templateManager.getTemplates());
            };

            Get["/getTemplatesAdminListIndex/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                return Response.AsJson(templateManager.getTemplates(index));
            };

            Put["/deleteTemplate"] = parameters =>
            {
                var id = this.Bind<long>();
                int result = templateManager.deleteTemplate(id);
                if (result == 1 || result == 0)
                    return Response.AsJson(result);

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };
            Put["/updateTemplate"] = parameters =>
            {
                var template = this.Bind<template>();

                if (templateManager.updateTemplate(template))
                {
                    return HttpStatusCode.OK;
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };
            /* ----- Auth Template -----*/


            Post["/addAuthTemplate"] = parameters =>
            {
                var temp = this.Bind<AuthTemplateManager.templateQuery>();
                AuthTemplateManager.templateQuery result = authTemplateManager.addTemplate(temp);
                if (result != null)
                {
                    return Response.AsJson(result);
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }

            };

            Get["/getAuthTemplatesAdmin"] = parameters =>
            {

                return Response.AsJson(authTemplateManager.getTemplates());
            };

            Get["/getAuthTemplatesAdminListIndex/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                return Response.AsJson(authTemplateManager.getTemplates(index));
            };

            Put["/deleteAuthTemplate"] = parameters =>
            {
                var id = this.Bind<int>();
                if (authTemplateManager.deleteTemplate(id))
                {
                    return HttpStatusCode.OK;
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };

            Put["/updateAuthTemplate"] = parameters =>
            {
                var template = this.Bind<authorizationtemplate>();

                if (authTemplateManager.updateTemplate(template))
                {
                    return HttpStatusCode.OK;
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };

            /* ----- Sponsor Complementary-----*/
            Post["/addSponsorComplementaryKeys"] = parameters =>
            {

                var obj = this.Bind<NancyService.Modules.SponsorManager.addComplementary>();
                return Response.AsJson(sponsorManager.addKeysTo(obj));

            };
            Put["/deleteComplementaryKey"] = parameters =>
            {
                var id = this.Bind<long>();
                if (sponsorManager.deleteComplementary(id))
                {
                    return HttpStatusCode.OK;
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };
            Put["/deleteSponsorComplementaryKey"] = parameters =>
            {
                var id = this.Bind<long>();
                return Response.AsJson(sponsorManager.deleteComplementarySponsor(id));
            };
            Get["/getComplementaryKeys"] = parameters =>
            {
                try
                {
                    // this.RequiresAuthentication();
                    // this.RequiresClaims(new[] { "minor" });
                    return Response.AsJson(sponsorManager.getComplementaryList());
                }
                catch { return null; }
            };
            Get["/getSponsorComplementaryKeys/{id:long}"] = parameters =>
            {
                try
                {
                    long id = parameters.id;
                    return Response.AsJson(sponsorManager.getSponsorComplementaryList(id));
                }
                catch { return null; }
            };

            Get["/getSponsorComplementaryKeysFromIndex/{index:int}/{id:long}"] = parameters =>
            {
                try
                {
                    NancyService.Modules.SponsorManager.ComplimentaryPagingQuery info = new NancyService.Modules.SponsorManager.ComplimentaryPagingQuery();
                    info.sponsorID = parameters.id;
                    info.index = parameters.index;
                    return Response.AsJson(sponsorManager.getSponsorComplementaryList(info));
                }
                catch { return null; }
            };

            Get["/searchKeyCodes/{index:int}/{id:long}/{criteria}"] = parameters =>
            {
                try
                {
                    NancyService.Modules.SponsorManager.ComplimentaryPagingQuery info = new NancyService.Modules.SponsorManager.ComplimentaryPagingQuery();
                    info.sponsorID = parameters.id;
                    info.index = parameters.index;
                    string criteria = parameters.criteria;
                    return Response.AsJson(sponsorManager.searchKeyCodes(info, criteria));
                }
                catch { return null; }
            };

            //--------------------------------------------Sponsor----------------------------

            Get["/getSponsorDeadline/"] = parameters =>
            {
                this.RequiresAuthentication();
                this.RequiresAnyClaim(new[] { "sponsor", "admin", "Master", "Admin", "CommitteEvaluator" });
                return Response.AsJson(sponsorManager.getSponsorDeadline());
            };

            Post["/addsponsor"] = parameters =>
            {

                var sponsor = this.Bind<NancyService.Modules.SponsorManager.SponsorQuery>();
                SponsorManager.SponsorQuery added = sponsorManager.addSponsor(sponsor);
                if (added != null)
                {
                    return Response.AsJson(added);
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };

            Get["/getSponsorListIndex/{index:int}"] = parameters =>
            {

                int index = parameters.index;
                return Response.AsJson(sponsorManager.getSponsorList(index));
            };

            Get["/getSponsorbyID/{id:long}"] = parameters =>
            {
                try
                {
                    this.RequiresAuthentication();
                    this.RequiresAnyClaim(new[] { "sponsor", "admin", "Master", "Admin", "CommitteEvaluator" });
                    long id = parameters.id;
                    return Response.AsJson(sponsorManager.getSponsorbyID(id));
                }
                catch { return null; }
            };


            Put["/updateSponsor"] = parameters =>
            {
                var sponsor = this.Bind<NancyService.Modules.SponsorManager.SponsorQuery>();
                SponsorManager.SponsorQuery s = sponsorManager.updateSponsor(sponsor);
                if (s != null)
                {
                    return Response.AsJson(s);
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };

            Get["/getSponsorTypesList"] = parameters =>
            {
                try
                {
                    //this.RequiresAuthentication();
                    //this.RequiresClaims(new[] { "admin" });
                    return Response.AsJson(sponsorManager.getSponsorTypesList());
                }
                catch { return null; }
            };


            Put["/deleteSponsor"] = parameters =>
            {
                var id = this.Bind<long>();
                if (sponsorManager.deleteSponsor(id))
                {
                    return HttpStatusCode.OK;
                }

                else
                {
                    return HttpStatusCode.Conflict;
                }
            };

            Get["/searchSponsors/{index:int}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                return Response.AsJson(sponsorManager.searchSponsors(index, criteria));
                //
            };

            /* ----- Topic -----(Heidi)*/

            //get list of conference topics
            Get["/getTopic"] = parameters =>
            {

                return Response.AsJson(topicManager.getTopicList());
            };

            //add a new topic
            Post["/addTopic"] = parameters =>
            {
                var topic = this.Bind<topiccategory>();
                return Response.AsJson(topicManager.addTopic(topic));
            };

            //update a new topic
            Put["/updateTopic"] = parameters =>
            {
                var topic = this.Bind<topiccategory>();
                return (topicManager.updateTopic(topic));
            };

            //delete topic
            Put["/deleteTopic/{topiccategoryID:int}"] = parameters =>
            {
                return topicManager.deleteTopic(parameters.topiccategoryID);
            };

            /* ----- Administrators -----(Heidi)*/

            //check if there an email has account in ConferenceAdmin when adding a new evaluator
            Get["/getNewAdmin/{email}"] = parameters =>
            {
                return adminManager.checkNewAdmin(parameters.email);
            };

            //get list of administratos
            Get["/getAdministrators/{index:int}"] = parameters =>
            {
                try
                {
                    int index = parameters.index;
                    return Response.AsJson(adminManager.getAdministratorList(index));
                }
                catch { return null; }
            };

            //get list of privileges in the system
            Get["/getPrivilegesList"] = parameters =>
            {
                try
                {
                    //this.RequiresAuthentication();
                    //this.RequiresClaims(new[] { "admin" });
                    return Response.AsJson(adminManager.getPrivilegesList());
                }
                catch { return null; }
            };

            //create a new administrator with a specified privilege
            Post["/addAdmin"] = parameters =>
            {
                var newAdmin = this.Bind<AdministratorQuery>();
                return Response.AsJson(adminManager.addAdmin(newAdmin));
            };

            //update an administrator
            Put["/editAdmin"] = parameters =>
            {
                var editAdmin = this.Bind<AdministratorQuery>();
                return Response.AsJson(adminManager.editAdministrator(editAdmin));
            };

            //remove administrator privileges to a user
            Put["/deleteAdmin"] = parameters =>
            {
                var delAdmin = this.Bind<AdministratorQuery>();
                return adminManager.deleteAdministrator(delAdmin);
            };

            //search administrators that contain the search criteria
            Get["/searchAdmin/{index:int}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                return Response.AsJson(adminManager.searchAdministrators(index, criteria));
            };

            /*------ Evaluators -----(Heidi)*/

            //get list of evaluators past applications
            Get["/getEvaluatorListFromIndex/{index:int}/{id:int}"] = parameters =>
            {
                int index = parameters.index;
                int id = parameters.id;
                return Response.AsJson(evaluatorManager.getEvaluatorList(index, id));
            };

            //get pending list of evaluators
            Get["/getPendingListFromIndex/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                return Response.AsJson(evaluatorManager.getPendingList(index));
            };

            //check evaluator has an account in ConferenceAdmin
            Get["/getNewEvaluator/{email}"] = parameters =>
            {
                return evaluatorManager.checkNewEvaluator(parameters.email);
            };

            //add a new evaluator with status "Accepted"
            Post["/addEvaluator/{email}"] = parameters =>
            {
                return evaluatorManager.addEvaluator(parameters.email);
            };

            //update status of an evaluator application
            Put["/updateEvaluatorAcceptanceStatus"] = parameters =>
            {
                var updateEvaluator = this.Bind<EvaluatorQuery>();
                return (evaluatorManager.updateAcceptanceStatus(updateEvaluator));
            };

            //search evaluators that contain search criteria
            Get["/searchEvaluators/{index:int}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                return Response.AsJson(evaluatorManager.searchEvaluators(index, criteria));
            };

            /* --------------------------------------- Registration ----------------------------------------*/

            Get["/getRegistrations/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                var list = registration.getRegistrationList(index);
                return Response.AsJson(list);
            };

            Get["/getUserTypes"] = parameters =>
            {
                List<UserTypeName> list = registration.getUserTypesList();
                return Response.AsJson(list);
            };

            Put["/updateRegistration"] = parameters =>
            {
                var registeredUser = this.Bind<RegisteredUser>();
                if (registration.updateRegistration(registeredUser))
                    return HttpStatusCode.OK;

                else
                    return HttpStatusCode.Conflict;
            };

            Delete["/deleteRegistration/{registrationID:int}"] = parameters =>
            {
                if (registration.deleteRegistration(parameters.registrationID))
                    return HttpStatusCode.OK;

                else
                    return HttpStatusCode.Conflict;
            };

            Post["/addRegistration"] = parameters =>
            {
                var user = this.Bind<user>();
                var reg = this.Bind<registration>();
                var mem = this.Bind<membership>();
                return Response.AsJson(registration.addRegistration(reg: reg, user: user, mem: mem));
            };

            Get["/getDates"] = parameters =>
            {
                List<string> list = registration.getDates();
                return Response.AsJson(list);
            };

            // [Randy] search within the list with a certain criteria
            Get["/searchRegistration/{index}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                var list = registration.searchRegistration(index, criteria);
                return Response.AsJson(list);
            };

            Get["/getAttendanceReport"] = parameters =>
            {
                return Response.AsJson(reportManager.getAttendanceReport());
            };

            //-------------------------------------GUESTS---------------------------------------------
            //Jaimeiris - Guest list for admins
            Get["/getGuestList/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                GuestsPagingQuery guestList = guest.getListOfGuests(index);

                if (guestList == null)
                {
                    guestList = new GuestsPagingQuery();
                }
                return Response.AsJson(guestList);
            };

            //Jaimeiris - update acceptance status of guest
            Put["/updateAcceptanceStatus"] = parameters =>
            {
                var update = this.Bind<AcceptanceStatusInfo>();
                int guestID = update.id;
                String acceptanceStatus = update.status;

                if (guest.updateAcceptanceStatus(guestID, acceptanceStatus)) return HttpStatusCode.OK;
                else return HttpStatusCode.Conflict;
            };

            //Jaimeiris - set registration status of guest to Rejected.
            Put["/rejectRegisteredGuest/{id}"] = parameters =>
            {
                int id = parameters.id;

                if (guest.rejectRegisteredGuest(id)) return HttpStatusCode.OK;
                else return HttpStatusCode.Conflict;
            };

            //Jaimeiris - get minor's authorizations
            Get["/displayAuthorizations/{id}"] = parameters =>
            {
                int id = parameters.id;
                List<MinorAuthorizations> authorizations = guest.getMinorAuthorizations(id);
                if (authorizations == null)
                {
                    authorizations = new List<MinorAuthorizations>();
                }
                return Response.AsJson(authorizations);
            };

            // [Randy] search within the list with a certain criteria
            Get["/searchGuest/{index}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                var list = guest.searchGuest(index, criteria);
                return Response.AsJson(list);
            };

            //-----------------------------------------WEBSITE CONTENT ----------------------------------[Heidi]

            //get content of the Home section of the website
            Get["/getHome"] = parameters =>
            {
                return Response.AsJson(webManager.getHome());
            };

            //get image found in the Home section of the website
            Get["/getHomeImage"] = parameters =>
            {
                return Response.AsJson(webManager.getHomeImage());
            };

            //get conference logo
            Get["/getWebsiteLogo"] = parameters =>
            {
                return Response.AsJson(webManager.getWebsiteLogo());
            };

            //Update content found in the Home section of the website
            Put["/saveHome"] = parameters =>
            {
                var home = this.Bind<HomeQuery>();
                return webManager.saveHome(home);
            };

            //remove an image from conference content
            Put["/removeFile/{data}"] = parameters =>
            {
                return webManager.removeFile(parameters.data);
            };

            //get content found in the Venue section of the website
            Get["/getVenue"] = parameters =>
            {
                return Response.AsJson(webManager.getVenue());
            };

            //update content found in the Venue section of the website
            Put["/saveVenue"] = parameters =>
            {
                var venue = this.Bind<VenueQuery>();
                return webManager.saveVenue(venue);
            };

            //get content found in the Contact section of the website
            Get["/getContact"] = parameters =>
            {
                return Response.AsJson(webManager.getContact());
            };

            //update content found in the Contact section of the website
            Put["/saveContact"] = parameters =>
            {
                var contact = this.Bind<ContactQuery>();
                return webManager.saveContact(contact);
            };

            //send inquire email 
            Post["/sendContactEmail"] = parameters =>
            {
                var emailInfo = this.Bind<ContactEmailQuery>();
                return Response.AsJson(webManager.sendContactEmail(emailInfo));
            };

            //get content found in the Call for Participation section of the website
            Get["/getParticipation"] = parameters =>
            {
                return Response.AsJson(webManager.getParticipation());
            };

            //update content found in the Call for Participation section of the website
            Put["/saveParticipation"] = parameters =>
            {
                var participation = this.Bind<ParticipationQuery>();
                return webManager.saveParticipation(participation);
            };

            //get content found in the Registration section of the website and registration fees
            Get["/getRegistrationInfo"] = parameters =>
            {
                return Response.AsJson(webManager.getRegistrationInfo());
            };

            //update content found in the Registration section of the website and registration fees
            Put["/saveRegistrationInfo"] = parameters =>
            {
                var registrationInfo = this.Bind<RegistrationQuery>();
                return webManager.saveRegistrationInfo(registrationInfo);
            };

            //get conference deadlines
            Get["/getDeadlines"] = parameters =>
            {
                return Response.AsJson(webManager.getDeadlines());
            };

            //get conference deadlines formatted to string: day of the week, day of the month, month and year
            Get["/getInterfaceDeadlines"] = parameters =>
            {
                return Response.AsJson(webManager.getInterfaceDeadlines());
            };

            //update conference deadlines
            Put["/saveDeadlines"] = parameters =>
            {
                var deadlines = this.Bind<DeadlinesQuery>();
                return webManager.saveDeadlines(deadlines);
            };

            //get content found in the Committee section of the website
            Get["/getCommitteeInterface"] = parameters =>
            {
                return Response.AsJson(webManager.getCommittee());
            };

            //update content found in the Committee section of the website
            Put["/saveCommitteeInterface"] = parameters =>
            {
                var info = this.Bind<CommitteeQuery>();
                return webManager.saveCommittee(info);
            };

            //get the benefits of a sponsor category
            Get["/getAdminSponsorBenefits/{data}"] = parameters =>
            {
                return webManager.getAdminSponsorBenefits(parameters.data);
            };

            //update benefits of a sponsor category
            Put["/saveAdminSponsorBenefits"] = parameters =>
            {
                var sponsor = this.Bind<SaveSponsorQuery>();
                return webManager.saveSponsorBenefits(sponsor);
            };

            //update content found in the Sponsor section of the website
            Put["/saveInstructions"] = parameters =>
            {
                var info = this.Bind<SponsorInterfaceBenefits>();
                return webManager.saveInstructions(info);
            };

            //get content found in the Sponsor section of the website
            Get["/getSponsorInstructions"] = parameters =>
            {
                return Response.AsJson(webManager.getInstructions());
            };

            //get all benefits for each category (for website content)
            Get["/getAllSponsorBenefits"] = parameters =>
            {
                return Response.AsJson(webManager.getAllSponsorBenefits());
            };

            //get conference general information: name, days
            Get["/getGeneralInfo"] = parameters =>
            {
                return Response.AsJson(webManager.getGeneralInfo());
            };

            //update conference general information: name, days, logo
            Put["/saveGeneralInfo"] = parameters =>
            {
                var info = this.Bind<GeneralInfoQuery>();
                return webManager.saveGeneralInfo(info);
            };

            //get documents found in the Program section of the website
            Get["/getProgram"] = parameters =>
            {
                return Response.AsJson(webManager.getProgram());
            };

            //get abstract document
            Get["/getAbstractDocument"] = parameters =>
            {
                return Response.AsJson(webManager.getAbstractDocument());
            };

            //get program document
            Get["/getProgramDocument"] = parameters =>
            {
                return Response.AsJson(webManager.getProgramDocument());
            };

            //update documents found in the Program section of the website
            Put["/saveProgram"] = parameters =>
            {
                var info = this.Bind<ProgramQuery>();
                return webManager.saveProgram(info);
            };

            //Get bill report including registrations and sponsor payments
            Get["/getBillReport"] = parameters =>
            {
                return Response.AsJson(reportManager.getBillReportList());
            };

            //search records in the bill report that contain the search criterai
            Get["/searchReport/{index:int}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                return Response.AsJson(reportManager.searchReport(index, criteria));
            };

            //get conference registrations
            Get["/getRegistrationPayments/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                return Response.AsJson(reportManager.getRegistrationPayments(index));
            };

            //get sponsor payments
            Get["/getSponsorPayments/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                return Response.AsJson(reportManager.getSponsorPayments(index));
            };
            //-----------------SUBMISSIONS- JAIMEIRIS------------------------------------
            //Jaimeiris - Gets all submissions in the system that have not been deleted
            Get["/getAllSubmissions/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                return Response.AsJson(submissionManager.getAllSubmissions(index));
            };
            //Jaimeiris - gets the evaluation for a submission
            Get["/getEvaluationsForSubmission/{submissionID}"] = parameters =>
            {
                long submissionID = parameters.submissionID;
                var evaluations = submissionManager.getSubmissionEvaluations(submissionID);

                return Response.AsJson(evaluations);
            };
            //Jaimeiris - gets all approved evaluators so as to assign them submissions to evaluate
            Get["/getAllEvaluators"] = parameters =>
            {
                return Response.AsJson(submissionManager.getAcceptedEvaluators());
            };
            //Jaimeiris - Assigns an evaluator to a submission
            Post["/assignEvaluator/{submissionID:long}/{evaluatorID:long}"] = parameters =>
            {
                long submissionID = parameters.submissionID;
                long evaluatorID = parameters.evaluatorID;

                Evaluation evList = submissionManager.assignEvaluator(submissionID, evaluatorID);

                return Response.AsJson(evList);
            };
            //Jaimeiris - Assigns a template to a submission
            Post["/assignTemplate/{submissionID:long}/{templateID:long}"] = parameters =>
            {
                long submissionID = parameters.submissionID;
                long templateID = parameters.templateID;
                if (submissionManager.assignTemplate(submissionID, templateID)) return HttpStatusCode.OK;
                else return HttpStatusCode.Conflict;
            };
            //Jaimeiris - Get the info of an evaluation
            Get["/getEvaluationDetails/{submissionID:long}/{evaluatorID:long}"] = parameters =>
            {
                long submissionID = parameters.submissionID;
                long evaluatorID = parameters.evaluatorID;
                Evaluation sub = submissionManager.getEvaluationDetails(submissionID, evaluatorID);
                if (sub == null)
                {
                    sub = new Evaluation();
                }
                return Response.AsJson(sub);
            };
            //Jaimeiris - Remove evaluator submission relation
            Put["/removeEvaluatorSubmission/{evaluatorSubmissionID}"] = parameters =>
            {
                long evaluatorSubmissionID = parameters.evaluatorSubmissionID;
                long es = submissionManager.removeEvaluatorSubmission(evaluatorSubmissionID);
                return Response.AsJson(es);
            };
            //Jaimeiris - Change submission status
            Put["/changeSubmissionStatus/{status}/{submissionID}"] = parameters =>
            {
                String newStatus = parameters.status;
                long submissionID = parameters.submissionID;
                Submission sub = submissionManager.changeSubmissionStatus(submissionID, newStatus);

                return Response.AsJson(sub);
            };
            //Jaimeiris - admin adds a submission
            Post["/postAdminSubmission"] = parameters =>
            {
                panel pannelToAdd = null;
                workshop workshopToAdd = null;
                submission submissionToAdd = this.Bind<submission>();
                usersubmission usersubTA = this.Bind<usersubmission>();

                int submissionTypeID = submissionToAdd.submissionTypeID;
                if (submissionTypeID == 3)
                {
                    pannelToAdd = this.Bind<panel>();
                }
                else if (submissionTypeID == 5)
                {
                    workshopToAdd = this.Bind<workshop>();
                }
                Submission newSubmission =
                    submissionManager.addSubmissionByAdmin(usersubTA, submissionToAdd, pannelToAdd, workshopToAdd);
                return Response.AsJson(newSubmission);
            };
            //Jaimeiris - post final version of evaluation submitted by admin
            Post["/postAdminFinalSubmission"] = parameters =>
            {
                panel pannelToAdd = null;
                workshop workshopToAdd = null;
                submission submissionToAdd = this.Bind<submission>();
                documentssubmitted submissionDocuments = this.Bind<documentssubmitted>();
                usersubmission usersubTA = this.Bind<usersubmission>();

                int submissionTypeID = submissionToAdd.submissionTypeID;
                if (submissionDocuments.document == null && submissionDocuments.documentName == null)
                {
                    submissionDocuments = null;
                }
                if (submissionTypeID == 3)
                {
                    pannelToAdd = this.Bind<panel>();
                }
                else if (submissionTypeID == 5)
                {
                    workshopToAdd = this.Bind<workshop>();
                }
                Submission newSubmission =
                    submissionManager.postAdminFinalSubmission(usersubTA, submissionToAdd, submissionDocuments, pannelToAdd, workshopToAdd);
                return Response.AsJson(newSubmission);
            };
            //Jaimeiris - gets all deleted submissions
            Get["/getDeletedSubmissions/{index:int}"] = parameters =>
            {
                int index = parameters.index;
                return Response.AsJson(submissionManager.getDeletedSubmissions(index));
            };
            //Jaimeiris - gets the details of a deleted submission
            Get["/getADeletedSubmission/{submissionID:long}"] = parameters =>
            {
                long submissionID = parameters.submissionID;
                return Response.AsJson(submissionManager.getADeletedSubmission(submissionID));
            };
            //Jaimeiris - gets the list of all users
            Get["/getListOfUsers"] = parameters =>
            {
                return Response.AsJson(submissionManager.getListOfUsers());
            };
            //Jaimeiris - returns true is the currently logged in user is the master
            Get["/isMaster/{userID:long}"] = parameters =>
            {
                long userID = parameters.userID;
                bool isMaster = submissionManager.isMaster(userID);
                return isMaster;
            };
            // [Randy] search within the list with a certain criteria
            Get["/searchSubmission/{index}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                var list = submissionManager.searchSubmission(index, criteria);
                return Response.AsJson(list);
            };
            // [Randy] search within the list with a certain criteria
            Get["/searchDeletedSubmission/{index}/{criteria}"] = parameters =>
            {
                int index = parameters.index;
                string criteria = parameters.criteria;
                var list = submissionManager.searchDeletedSubmission(index, criteria);
                return Response.AsJson(list);
            };
            //Jaimeiris - Gets the file for the submission with submissionID
            Get["/getSubmissionFile/{fileID}"] = parameters =>
            {
                int fileID = parameters.fileID;
                return Response.AsJson(submissionManager.getSubmissionFile(fileID));
            };
            //Jaimeiris - get submission report
            Get["/getSubmissionsReport"] = parameters =>
            {
                return Response.AsJson(reportManager.getSubmissionsReport());
            };
            //Jaimeiris-get templates list
            Get["/getTemplates"] = parameters =>
                {
                    return Response.AsJson(submissionManager.getTemplates());
                };

            //------------------------------------Banner---------------------------------------------
            Get["/getBanners/{index:int}/{sponsor}"] = parameters =>
            {
                int index = parameters.index;
                String sponsor = parameters.sponsor;
                return Response.AsJson(bannerManager.getBannerList(sponsor, index));
            };

        }
    }
    public class AcceptanceStatusInfo
    {
        public int id { get; set; }
        public String status { get; set; }
    }
}