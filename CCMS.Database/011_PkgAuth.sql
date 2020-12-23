-------------------------------------------------------------------------
-- PKG_AUTH
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_auth IS
-------------------------------------------------------------------------
    PROCEDURE p_get_auth_details (
        pi_user_email   IN app_users.user_email%type,
        po_user_id      OUT app_users.user_id%type,
        po_password     OUT app_users.user_password%type,
        po_salt         OUT app_users.password_salt%type
    );

    PROCEDURE p_create_session (
        pi_user_id      IN user_sessions.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type,
        pi_guid         IN user_sessions.guid%type
    );
    
    FUNCTION f_get_user_roles (
        pi_user_id  IN app_users.user_id%type
    ) RETURN VARCHAR2;

    PROCEDURE p_increment_login_count (
        pi_user_id      IN app_users.user_id%type
    );

    PROCEDURE p_login (
        pi_user_id      IN app_users.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type,
        pi_guid         IN user_sessions.guid%type,
        po_roles        OUT VARCHAR2
    );
    
    PROCEDURE p_is_valid_session (
        pi_user_id      IN user_sessions.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type,
        pi_guid         IN user_sessions.guid%type,
        po_is_valid     OUT NUMBER
    );

    PROCEDURE p_logout (
        pi_user_id      IN app_users.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type
    );

END pkg_auth;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_auth IS
-------------------------------------------------------------------------
    PROCEDURE p_get_auth_details (
        pi_user_email   IN app_users.user_email%type,
        po_user_id      OUT app_users.user_id%type,
        po_password     OUT app_users.user_password%type,
        po_salt         OUT app_users.password_salt%type
    ) IS
    BEGIN
        select
            case
                when account_locked is not null then 0
                else user_id
            end,
            user_password,
            password_salt
        into
            po_user_id,
            po_password,
            po_salt
        from
            app_users
        where   user_email = pi_user_email
            and deleted is null;
    EXCEPTION
        when no_data_found then
            po_user_id := -1;
            po_password := null;
            po_salt := null;
        when others then
            raise_application_error(
                -20001,
                'p_get_auth_details - pi_user_email: ' || pi_user_email
                || chr(10) || sqlerrm);
    END p_get_auth_details;
-------------------------------------------------------------------------
    PROCEDURE p_create_session (
        pi_user_id      IN user_sessions.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type,
        pi_guid         IN user_sessions.guid%type
    ) IS
    BEGIN
        update user_sessions
        set guid = pi_guid,
            started_at = sysdate
        where user_id = pi_user_id
        and platform_id = pi_platform_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_session - pi_user_id: ' || pi_user_id
                || '; pi_platform_id: ' || pi_platform_id
                || '; pi_guid: ' || pi_guid
                || chr(10) || sqlerrm);
    END p_create_session;
-------------------------------------------------------------------------
    FUNCTION f_get_user_roles (
        pi_user_id  IN app_users.user_id%type
    ) RETURN VARCHAR2 IS
        v_roles_num app_users.user_roles%type;
        v_roles_str VARCHAR2(4000);
    BEGIN
        select
            user_roles
        into
            v_roles_num
        from
            app_users
        where   user_id = pi_user_id
            and deleted is null;

        select
            listagg (
				role_name,
            ',') within group
            (order by role_id)
        into
            v_roles_str
        from
            app_roles
        where	deleted is null
			and bitand(v_roles_num, role_id) = role_id;
        return v_roles_str;
    EXCEPTION
        when no_data_found then
            return null;
        when others then
            raise_application_error(
                -20001,
                'f_get_user_roles - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END f_get_user_roles;
-------------------------------------------------------------------------
    PROCEDURE p_increment_login_count (
        pi_user_id      IN app_users.user_id%type
    ) IS
        v_curr_count    app_users.login_count%type;
    BEGIN
        select
            login_count
        into
            v_curr_count
        from
            app_users
        where
            user_id = pi_user_id;

        if v_curr_count > 3 then
            pkg_app_users.p_lock_account (pi_user_id);
            return;
        end if;

        update app_users
        set login_count = v_curr_count + 1
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_increment_login_count - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END p_increment_login_count;
-------------------------------------------------------------------------
    PROCEDURE p_login (
        pi_user_id      IN app_users.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type,
        pi_guid         IN user_sessions.guid%type,
        po_roles        OUT VARCHAR2
    ) IS
    BEGIN
        p_create_session(pi_user_id, pi_platform_id, pi_guid);
        po_roles := f_get_user_roles(pi_user_id);

        update app_users
        set login_count = 0
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_login - pi_user_id: ' || pi_user_id
                || '; pi_platform_id: ' || pi_platform_id
                || '; pi_guid: ' || pi_guid
                || chr(10) || sqlerrm);
    END p_login;
-------------------------------------------------------------------------
    PROCEDURE p_is_valid_session (
        pi_user_id      IN user_sessions.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type,
        pi_guid         IN user_sessions.guid%type,
        po_is_valid     OUT NUMBER
    ) IS
    BEGIN
        select
            1
        into
            po_is_valid
        from
            user_sessions
        where   deleted is null
            and guid is not null
            and user_id = pi_user_id
            and platform_id = pi_platform_id
            and guid = pi_guid;
    EXCEPTION
        when no_data_found then
            po_is_valid := 0;
        when others then
            raise_application_error(
                -20001,
                'p_is_valid_session - pi_user_id: ' || pi_user_id
                || '; pi_platform_id: ' || pi_platform_id
                || '; pi_guid: ' || pi_guid
                || chr(10) || sqlerrm);
    END p_is_valid_session;
-------------------------------------------------------------------------
    PROCEDURE p_logout (
        pi_user_id      IN app_users.user_id%type,
        pi_platform_id  IN user_sessions.platform_id%type
    ) IS
    BEGIN
        update user_sessions
        set guid = null,
            started_at = null
        where   user_id = pi_user_id
            and platform_id = pi_platform_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_logout - pi_user_id: ' || pi_user_id
                || '; pi_platform_id: ' || pi_platform_id
                || chr(10) || sqlerrm);
    END p_logout;
-------------------------------------------------------------------------
END pkg_auth;
/