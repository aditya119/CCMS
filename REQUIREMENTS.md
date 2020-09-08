### Requirements

---

**CCMS** refers to **Court Case Monitoring System**, it provides a platform to track and monitor court cases digitally.

---

Functional Requirements for CCMS are as follows:

1. **Login functionality**:
	1. CCMS will be supported on 3 platforms, i.e., Web, Mobile, and Desktop app.
	2. System shall allow user to login from any platform, given that they provide valid credentials in the form of username and password.
	3. System shall allow only one login per platform.
		e.g. If a user attempts multiple simultaneous logins on the web platform, then only the latest session would be considered alive.

---

2. **Case Creation functionality**:
	1. System shall allow users to create new cases.
	2. System shall ensure that users enter mandatory information before saving a case. Mandatory information shall be:
		- File Number
		- Case Number
		- Court Name
		- Case Type
		- Location
		- Year
		- Lawyer
		- Date on which case was filed
		- Date on which the user's organisation received the notice
		- Date of first hearing of the case
		- Petitioner Name
		- Respondent Name
	3. System shall allow users to add proceeding information for each case.
	4. System shall allow 'Manager' users to assign a proceeding to any user.
	5. System shall automatically assign 'Operator' users any proceedings that they add.
	6. Every case will have by default have one proceeding with Proceeding Date set as Date of first hearing of the case and judgment as pending.
	7. System shall ensure that dates are in correct order.
	8. System shall allow users to select a different lawyer for each proceeding.

---

3. **Case Updation functionality**:
	1. System shall allow users to add judgments for Pending proceedings and add new ones.
	2. System shall not allow 'Operator' users to update other case data or closed proceedings.
	3. System shall allow 'Operator' users to update data if that was empty initially. e.g. Petitioner e-mail address was not entered initially.
	4. System shall allow 'Manager' users to update any data.
	5. System shall allow creation of an appeal of case, provided that its latest proceeding has Decision as 'Final Judgement'.
	6. System shall allow users to download any documents uploaded for the case.

---

4. **Action Items functionality**:
	1. System shall display list of cases with pending Proceedings assigned to the logged in user.
	2. System shall display this list in ascending order of Proceeding Date.
	3. System shall display cases whose Proceeding Date has crossed in red colour.
	4. System shall allow 'Manager' user to see cases assigned to others as well.

---

5. **Notifications functionality**:
	1. System shall display upcoming cases along with the assigned user.
	2. System shall display upcoming cases in 3 windows, 1, 7, and 15 days, respectively.
	3. System shall send e-mail and SMS reminders to the assigned users of the case 7 days before the Proceeding Date.
	4. System shall send e-mail and SMS notifications to 'Manager' users when proceeding data has not been updated for 5 days after the Proceeding Date.

---

6. **Parameterised Reports functionality**:
	1. System shall allow user to generate reports based on the following parameters:
		- Location
		- Court
		- Lawyer
		- Next Hearing Date between
	2. System shall also allow users to generate reports directly from File Number or Case Number.
	3. System shall ensure that a row in the report shall represent a single proceeding.
	4. System shall display following data in the tabular report:
		- File Number
		- Case Number
		- Appeal Number
		- Court Name
		- Year
		- Lawyer
		- Proceeding Date
		- Decision
		- Next Hearing On
	5. System shall provide clear distinction between multiple proceedings and appeals of a case.
	6. System shall allow user to download Excel file of the report.
	7. System shall allow user to download uploaded court orders for the selected proceeding.
	8. System shall allow user to download a PDF report for a single case.

---

7. **Configuration functionality**:
	1. System shall allow user to update their password.
	2. System shall allow 'Administrator' users to create new user accounts, and update existing ones.
	3. System shall allow 'Administrator' users to configure Lawyer, Court, Location, and Case Type details.
