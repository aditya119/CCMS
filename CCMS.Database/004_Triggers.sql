-------------------------------------------------------------------------
-- Create triggers
-------------------------------------------------------------------------

create or replace trigger t_new_user_created
after insert on app_users
for each row
begin
    insert into user_sessions (
        user_id
    ) values (
        :new.user_id
    );
end t_new_user_created;
/
