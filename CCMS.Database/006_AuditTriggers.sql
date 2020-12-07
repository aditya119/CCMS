-------------------------------------------------------------------------
-- Create Triggers for Audit
-------------------------------------------------------------------------

create or replace trigger t_audit_update_attachments
before update on attachments
for each row
begin
	if :old.filename <> :new.filename then
        p_audit_log('attachments', :new.attachment_id, 'filename', :old.filename, :new.filename, :new.last_update_by);
    end if;
	if dbms_lob.compare(:old.attachment_file, :new.attachment_file) <> 0 then
        p_audit_log('attachments', :new.attachment_id, 'attachment_file', 'Old File', 'New File', :new.last_update_by);
    end if;
	if :old.deleted <> :new.deleted then
        p_audit_log('attachments', :new.attachment_id, 'deleted', :old.deleted, :new.deleted, :new.last_update_by);
    end if;
end t_audit_update_attachments;
/

create or replace trigger t_audit_update_case
before update on court_cases
for each row
begin
	if :old.case_number <> :new.case_number then
        p_audit_log('court_cases', :new.case_id, 'case_number', :old.case_number, :new.case_number, :new.last_update_by);
    end if;
	if :old.appeal_number <> :new.appeal_number then
        p_audit_log('court_cases', :new.case_id, 'appeal_number', :old.appeal_number, :new.appeal_number, :new.last_update_by);
    end if;
	if :old.case_type_id <> :new.case_type_id then
        p_audit_log('court_cases', :new.case_id, 'case_type_id', :old.case_type_id, :new.case_type_id, :new.last_update_by);
    end if;
	if :old.court_id <> :new.court_id then
        p_audit_log('court_cases', :new.case_id, 'court_id', :old.court_id, :new.court_id, :new.last_update_by);
    end if;
	if :old.location_id <> :new.location_id then
        p_audit_log('court_cases', :new.case_id, 'location_id', :old.location_id, :new.location_id, :new.last_update_by);
    end if;
	if :old.lawyer_id <> :new.lawyer_id then
        p_audit_log('court_cases', :new.case_id, 'lawyer_id', :old.lawyer_id, :new.lawyer_id, :new.last_update_by);
    end if;
	if :old.deleted <> :new.deleted then
        p_audit_log('court_cases', :new.case_id, 'deleted', :old.deleted, :new.deleted, :new.last_update_by);
    end if;
end t_audit_update_case;
/

create or replace trigger t_audit_update_casedates
before update on case_dates
for each row
begin
	if :old.case_filed_on <> :new.case_filed_on then
        p_audit_log('case_dates', :new.case_id, 'case_filed_on', :old.case_filed_on, :new.case_filed_on, :new.last_update_by);
    end if;
	if :old.notice_received_on <> :new.notice_received_on then
        p_audit_log('case_dates', :new.case_id, 'notice_received_on', :old.notice_received_on, :new.notice_received_on, :new.last_update_by);
    end if;
	if :old.first_hearing_on <> :new.first_hearing_on then
        p_audit_log('case_dates', :new.case_id, 'first_hearing_on', :old.first_hearing_on, :new.first_hearing_on, :new.last_update_by);
    end if;
	if :old.deleted <> :new.deleted then
        p_audit_log('case_dates', :new.case_id, 'deleted', :old.deleted, :new.deleted, :new.last_update_by);
    end if;
end t_audit_update_casedates;
/

create or replace trigger t_audit_update_caseactors
before update on case_actors
for each row
begin
	if :old.actor_type_id <> :new.actor_type_id then
        p_audit_log('case_actors', :new.case_actor_id, 'case_filed_on', :old.actor_type_id, :new.actor_type_id, :new.last_update_by);
    end if;
	if :old.actor_name <> :new.actor_name then
        p_audit_log('case_actors', :new.case_actor_id, 'actor_name', :old.actor_name, :new.actor_name, :new.last_update_by);
    end if;
	if :old.actor_address <> :new.actor_address then
        p_audit_log('case_actors', :new.case_actor_id, 'actor_address', :old.actor_address, :new.actor_address, :new.last_update_by);
    end if;
	if :old.actor_email <> :new.actor_email then
        p_audit_log('case_actors', :new.case_actor_id, 'actor_email', :old.actor_email, :new.actor_email, :new.last_update_by);
    end if;
	if :old.actor_phone <> :new.actor_phone then
        p_audit_log('case_actors', :new.case_actor_id, 'actor_phone', :old.actor_phone, :new.actor_phone, :new.last_update_by);
    end if;
	if :old.detail_file <> :new.detail_file then
        p_audit_log('case_actors', :new.case_actor_id, 'detail_file', :old.detail_file, :new.detail_file, :new.last_update_by);
    end if;
	if :old.deleted <> :new.deleted then
        p_audit_log('case_actors', :new.case_actor_id, 'deleted', :old.deleted, :new.deleted, :new.last_update_by);
    end if;
end t_audit_update_casedates;
/

create or replace trigger t_audit_update_caseproceedings
before update on case_proceedings
for each row
begin
	if :old.proceeding_date <> :new.proceeding_date then
        p_audit_log('case_proceedings', :new.case_proceeding_id, 'proceeding_date', :old.proceeding_date, :new.proceeding_date, :new.last_update_by);
    end if;
	if :old.proceeding_decision <> :new.proceeding_decision then
        p_audit_log('case_proceedings', :new.case_proceeding_id, 'proceeding_decision', :old.proceeding_decision, :new.proceeding_decision, :new.last_update_by);
    end if;
	if :old.next_hearing_on <> :new.next_hearing_on then
        p_audit_log('case_proceedings', :new.case_proceeding_id, 'next_hearing_on', :old.next_hearing_on, :new.next_hearing_on, :new.last_update_by);
    end if;
	if :old.judgement_file <> :new.judgement_file then
        p_audit_log('case_proceedings', :new.case_proceeding_id, 'judgement_file', :old.judgement_file, :new.judgement_file, :new.last_update_by);
    end if;
	if :old.assigned_to <> :new.assigned_to then
        p_audit_log('case_proceedings', :new.case_proceeding_id, 'assigned_to', :old.assigned_to, :new.assigned_to, :new.last_update_by);
    end if;
	if :old.deleted <> :new.deleted then
        p_audit_log('case_proceedings', :new.case_proceeding_id, 'deleted', :old.deleted, :new.deleted, :new.last_update_by);
    end if;
end t_audit_update_caseproceedings;
/