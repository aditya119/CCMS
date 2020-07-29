A case, uniquely identified by a File Number, has the following properties:
	1) Court Name (High Court, Supreme Court, etc.)
	2) Case Type (Civil, CWP, Criminal, etc.)
	3) Location (City in which the case was filed)
	4) Case Number
	5) Year
	6) Lawyer (the one handling the case on your behalf)
	7) Date on which case was filed
	8) Date on which the user's organisation received the notice
	9) Date of first hearing of the case
	10) Petitioner Details (Name, E-mail, Address)
	11) Respondent Details (Name, E-mail, Address)
The user's organisation would be either petitioner or respondent for the case.
A detail file can be attached to either petitioner, respondent, or both.

Court Name, Case Type, Location, and Lawyer should be configurable.
Configuration rights should be given to priviliged users.

Each case can have multiple proceedings.
Every proceeding for a case has a Proceeding Date.
The decision of the proceeding can be:
	1) Adjournment - This requires date of the Next Hearing which becomes the Proceeding Date for the next proceeding.
	2) Interim Order - This requires date of the Next Hearing and also a file of the Order judgement needs to be uploaded.
	3) Final Judgement - This is the final proceeding of a case, after which the case is closed. It requires a judgement file to be uploaded.
The default decision for a new case would be Pending, which would only have a value of the upcoming Proceeding Date. It will have no other data.

User may add a description for each proceeding.

A case may be appealed after a Final Judgement is received.
An appeal is essentially a new case which shares the file number. It will then have proceedings of its own.

A case is Pending if the decision of its latest Proceeding is Pending or Adjournment.
A case is Disposed Off if the decision of its latest Proceeding is Final Judgement.

A notification would be needed for cases with Next Hearing Dates approaching 1, 7, and 15 days.

Tabular Reports for cases would be required.
Reports can be filter by:
	1) Location
	2) Court
	3) Lawyer
	4) Next Hearing between
	5) Case Number
	6) File Number

User should also be able generate reports for an individual case, and download files uploaded for each order or Petitioner/Respondent.

Priviliged Users should be able to delete a case.