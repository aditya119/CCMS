-------------------------------------------------------------------------
-- Add Factory Data
-------------------------------------------------------------------------

begin
	insert into app_users (
		user_id,
        user_email,
        user_fullname,
        user_password,
        password_salt,
        is_sys_admin,
        is_manager,
        is_operator
	) values (
        1,
        'admin',
        'Administrator',
        'manager',-- Todo: add correct password
        'manager',
        1,
        1,
        1
    );
exception
    when others then
        raise_application_error(-20001, 'app_users - Unable to add factory data' || chr(10) || sqlerrm);
end;
/

begin
	insert into proceeding_decisions (
		proceeding_decision_id,
        proceeding_decision_name,
        has_next_hearing_date,
        has_order_attachment
	)
    select 0, 'PENDING', 0, 0 from dual
    union all
    select 1, 'ADJOURNMENT', 1, 0 from dual
    union all
    select 2, 'INTERIM ORDER', 1, 1 from dual
    union all
    select 3, 'FINAL JUDGEMENT', 0, 1 from dual;
exception
    when others then
        raise_application_error(-20001, 'proceeding_decisions - Unable to add factory data' || chr(10) || sqlerrm);
end;
/

begin
	insert into actor_types (
        actor_type_id,
        actor_type_name
	)
    select 1, 'PETITIONER' from dual
    union all
    select 2, 'RESPONDENT' from dual;
exception
    when others then
        raise_application_error(-20001, 'actor_types - Unable to add factory data' || chr(10) || sqlerrm);
end;
/

begin
	insert into lawyers (
        lawyer_id,
        lawyer_fullname,
        lawyer_email,
        lawyer_phone,
        lawyer_address
	) values (
        0,
        '-- No Lawyer selected --',
        'No Lawyer selected',
        'No Lawyer selected',
        'No Lawyer selected'
    );
exception
    when others then
        raise_application_error(-20001, 'lawyers - Unable to add factory data' || chr(10) || sqlerrm);
end;
/

begin
	insert into courts (
        court_id,
        court_name
	) values (
        0,
        '-- NO COURT SELECTED --'
    );
exception
    when others then
        raise_application_error(-20001, 'courts - Unable to add factory data' || chr(10) || sqlerrm);
end;
/

begin
	insert into case_types (
        case_type_id,
        case_type_name
	) values (
        0,
        '-- NO CASE-TYPE SELECTED --'
    );
exception
    when others then
        raise_application_error(-20001, 'case_types - Unable to add factory data' || chr(10) || sqlerrm);
end;
/

begin
	insert into locations (
        location_id,
        location_name
	) values (
        0,
        '-- NO LOCATION SELECTED --'
    );
exception
    when others then
        raise_application_error(-20001, 'locations - Unable to add factory data' || chr(10) || sqlerrm);
end;
/

begin
	insert into attachments (
        attachment_id,
        filename
	) values (
        0,
        'No File Attached'
    );
exception
    when others then
        raise_application_error(-20001, 'attachments - Unable to add factory data' || chr(10) || sqlerrm);
end;
/