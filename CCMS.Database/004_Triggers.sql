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
        last_update_by
    )
    select
        :new.case_id,
        actor_type_id,
        'No ' || actor_type_name || ' added',
        :new.last_update_by
    from
        actor_types
    where deleted is null;
end t_new_case_created;
/
