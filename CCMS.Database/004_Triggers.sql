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
