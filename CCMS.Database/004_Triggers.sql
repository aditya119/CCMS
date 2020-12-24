-------------------------------------------------------------------------
-- Create triggers
-------------------------------------------------------------------------

create or replace trigger t_new_user_created
after insert on app_users
for each row
begin
    insert into user_sessions (
        user_id,
        platform_id
    )
    select
        :new.user_id,
        platform_id
    from
        platforms
    where
        deleted is null;
end t_new_user_created;
/

create or replace trigger t_new_case_created
after insert on court_cases
for each row
begin
    insert into case_dates (
        case_id,
        last_update_by
    ) values (
        :new.case_id,
        :new.last_update_by
    );
    insert into case_actors (
        case_id,
        actor_type_id,
        actor_name,
        detail_file,
        last_update_by
    )
    select
        :new.case_id,
        actor_type_id,
        'No ' || actor_type_name || ' added',
        0,
        :new.last_update_by
    from
        actor_types
    where deleted is null;
end t_new_case_created;
/

create or replace trigger t_first_hearing_added
after update on case_dates
for each row
begin
    if :old.first_hearing_on is null and :new.first_hearing_on is not null then
        insert into case_proceedings (
            case_id,
            proceeding_date,
            proceeding_decision,
            judgement_file,
            assigned_to,
            last_update_by
        ) values (
            :new.case_id,
            :new.first_hearing_on,
            0, -- Pending
            0, -- No file attached
            :new.last_update_by,
            :new.last_update_by
        );
    end if;
end t_first_hearing_added;
/

create or replace trigger t_proceeding_updated
after update on case_proceedings
for each row
begin
    if :old.next_hearing_on is null and :new.next_hearing_on is not null then
        update case_proceedings
        set is_latest_proceeding = 0
        where case_proceeding_id = :old.case_proceeding_id;

        insert into case_proceedings (
            case_id,
            proceeding_date,
            proceeding_decision,
            judgement_file,
            assigned_to,
            last_update_by
        ) values (
            :new.case_id,
            :new.next_hearing_on,
            0, -- Pending
            0, -- No file attached
            :new.assigned_to,
            :new.last_update_by
        );
    end if;
    if :old.proceeding_decision <> :new.proceeding_decision then
        update court_cases
        set case_status = :new.proceeding_decision
        where case_id = :new.case_id;
    end if;
end t_proceeding_updated;
/