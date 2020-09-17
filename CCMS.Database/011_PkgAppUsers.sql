-------------------------------------------------------------------------
-- PKG_APP_USERS: Perform CRUD operations on APP_USERS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_app_users IS

    PROCEDURE p_get_all_users (
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_get_users_with_roles (
        pi_roles    IN app_users.user_roles%type,
        po_cursor   OUT sys_refcursor
    );
    
    PROCEDURE p_get_users_with_roles_str (
        pi_roles_str    IN VARCHAR2,
        po_cursor       OUT sys_refcursor
    );

    PROCEDURE p_get_user_details (
        pi_user_id  IN app_users.user_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_exists_user (
        pi_user_email   IN app_users.user_email%type,
        po_user_id      OUT app_users.user_id%type,
        po_deleted      OUT app_users.deleted%type
    );

    PROCEDURE p_create_new_user (
        pi_user_email       IN app_users.user_email%type,
        pi_user_fullname    IN app_users.user_fullname%type,
        pi_user_password    IN app_users.user_password%type,
        pi_password_salt    IN app_users.password_salt%type,
        pi_user_roles       IN app_users.user_roles%type,
        po_user_id          OUT app_users.user_id%type
    );

    PROCEDURE p_update_user (
        pi_user_id          IN app_users.user_id%type,
        pi_user_email       IN app_users.user_email%type,
        pi_user_fullname    IN app_users.user_fullname%type,
        pi_user_roles       IN app_users.user_roles%type
    );

    PROCEDURE p_change_password (
        pi_user_id          IN app_users.user_id%type,
        pi_user_password    IN app_users.user_password%type
    );

    PROCEDURE p_delete_user (
        pi_user_id  IN app_users.user_id%type
    );

END pkg_app_users;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_app_users IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_users (
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                user_id,
                user_fullname || ' (' || user_email || ')'
            from
                app_users
            where
                deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_users' || chr(10) || sqlerrm);
    END p_get_all_users;
-------------------------------------------------------------------------
    PROCEDURE p_get_users_with_roles (
        pi_roles    IN app_users.user_roles%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                user_id,
                user_fullname || ' (' || user_email || ')'
            from
                app_users
            where   deleted is null
                and bitand(user_roles, pi_roles) = pi_roles;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_users_with_roles - pi_roles: ' || pi_roles
                || chr(10) || sqlerrm);
    END p_get_users_with_roles;
-------------------------------------------------------------------------
    PROCEDURE p_get_users_with_roles_str (
        pi_roles_str    IN VARCHAR2,
        po_cursor       OUT sys_refcursor
    ) IS
        v_roles_id  app_users.user_roles%type;
    BEGIN
        pkg_roles.p_get_role_id(pi_roles_str, v_roles_id);
        p_get_users_with_roles(v_roles_id, po_cursor);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_users_with_roles_str - pi_roles_str: ' || pi_roles_str
                || chr(10) || sqlerrm);
    END p_get_users_with_roles_str;
-------------------------------------------------------------------------
    PROCEDURE p_get_user_details (
        pi_user_id  IN app_users.user_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                user_id,
                user_email,
                user_fullname,
                user_roles
            from
                app_users
            where
                user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_user_details - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END p_get_user_details;
-------------------------------------------------------------------------
    PROCEDURE p_exists_user (
        pi_user_email   IN app_users.user_email%type,
        po_user_id      OUT app_users.user_id%type,
        po_deleted      OUT app_users.deleted%type
    ) IS
    BEGIN
        select
            user_id,
            deleted
        into
            po_user_id,
            po_deleted
        from
            app_users
        where
            user_email = lower(pi_user_email);
    EXCEPTION
        when no_data_found then
            po_user_id := null;
            po_deleted := null;
        when others then
            raise_application_error(
                -20001,
                'p_exists_user - pi_user_email: ' || pi_user_email
                || chr(10) || sqlerrm);
    END p_exists_user;
-------------------------------------------------------------------------
    PROCEDURE p_create_new_user (
        pi_user_email       IN app_users.user_email%type,
        pi_user_fullname    IN app_users.user_fullname%type,
        pi_user_password    IN app_users.user_password%type,
        pi_password_salt    IN app_users.password_salt%type,
        pi_user_roles       IN app_users.user_roles%type,
        po_user_id          OUT app_users.user_id%type
    ) IS
        v_deleted   app_users.deleted%type;
    BEGIN
        p_exists_user(pi_user_email, po_user_id, v_deleted);
        if v_deleted is not null then
            update app_users
            set deleted = null
            where user_id = po_user_id;

            p_update_user (
                po_user_id,
                pi_user_email,
                pi_user_fullname,
                pi_user_roles
            );
            return;
        end if;
        insert into app_users (
            user_email,
            user_fullname,
            user_password,
            password_salt,
            user_roles
        ) values (
            lower(pi_user_email),
            pi_user_fullname,
            pi_user_password,
            pi_password_salt,
            pi_user_roles
        );
        select
            user_id
            into
            po_user_id
        from
            app_users
        where
            user_email = lower(pi_user_email);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_new_user - pi_user_email: ' || pi_user_email
                || '; pi_user_fullname: ' || pi_user_fullname
                || '; pi_user_password: ' || pi_user_password
                || '; pi_password_salt: ' || pi_password_salt
                || '; pi_user_roles: ' || pi_user_roles
                || chr(10) || sqlerrm);
    END p_create_new_user;
-------------------------------------------------------------------------
    PROCEDURE p_update_user (
        pi_user_id          IN app_users.user_id%type,
        pi_user_email       IN app_users.user_email%type,
        pi_user_fullname    IN app_users.user_fullname%type,
        pi_user_roles       IN app_users.user_roles%type
    ) IS
    BEGIN
        update app_users
        set user_email = lower(pi_user_email),
            user_fullname = pi_user_fullname,
            user_roles = pi_user_roles
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_user - pi_user_id: ' || pi_user_id
                || '; pi_user_email: ' || pi_user_email
                || '; pi_user_fullname: ' || pi_user_fullname
                || '; pi_user_roles: ' || pi_user_roles
                || chr(10) || sqlerrm);
    END p_update_user;
-------------------------------------------------------------------------
    PROCEDURE p_change_password (
        pi_user_id          IN app_users.user_id%type,
        pi_user_password    IN app_users.user_password%type
    ) IS
    BEGIN
        update app_users
        set user_password = pi_user_password
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_change_password - pi_user_id: ' || pi_user_id
                || '; pi_user_password: ' || pi_user_password
                || chr(10) || sqlerrm);
    END p_change_password;
-------------------------------------------------------------------------
    PROCEDURE p_delete_user (
        pi_user_id  IN app_users.user_id%type
    ) IS
    BEGIN
        update app_users
        set deleted = sysdate
        where user_id = pi_user_id;
        
        update user_sessions
        set deleted = sysdate
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_user - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END p_delete_user;
-------------------------------------------------------------------------
END pkg_app_users;
/