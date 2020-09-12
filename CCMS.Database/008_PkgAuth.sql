-------------------------------------------------------------------------
-- PKG_AUTH
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_auth IS

    PROCEDURE p_get_auth_details (
        pi_user_email   IN app_users.user_email%type,
        po_user_id      OUT app_users.user_id%type,
        po_password     OUT app_users.user_password%type,
        po_salt         OUT app_users.password_salt%type
    );

    PROCEDURE p_create_web_session (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type
    );
    
    PROCEDURE p_create_mobile_session (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type
    );
    
    PROCEDURE p_create_desktop_session (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type
    );
    
    FUNCTION f_get_roles (
        pi_user_id  IN app_users.user_id%type
    ) RETURN VARCHAR2;

    PROCEDURE p_login (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type,
        pi_platform IN VARCHAR2, --WEB,MOBILE,DESKTOP
        po_roles    OUT VARCHAR2
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
            user_id,
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
            po_user_id := null;
            po_password := null;
            po_salt := null;
        when others then
            raise_application_error(
                -20001,
                'p_get_auth_details - pi_user_email: ' || pi_user_email
                || chr(10) || sqlerrm);
    END p_get_auth_details;
-------------------------------------------------------------------------
    PROCEDURE p_create_web_session (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type
    ) IS
    BEGIN
        update user_sessions
        set web_guid = pi_guid
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_web_session - pi_user_id: ' || pi_user_id
                || '; pi_guid: ' || pi_guid
                || chr(10) || sqlerrm);
    END p_create_web_session;
-------------------------------------------------------------------------
    PROCEDURE p_create_mobile_session (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type
    ) IS
    BEGIN
        update user_sessions
        set mobile_guid = pi_guid
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_mobile_session - pi_user_id: ' || pi_user_id
                || '; pi_guid: ' || pi_guid
                || chr(10) || sqlerrm);
    END p_create_mobile_session;
-------------------------------------------------------------------------
    PROCEDURE p_create_desktop_session (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type
    ) IS
    BEGIN
        update user_sessions
        set desktop_guid = pi_guid
        where user_id = pi_user_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_create_desktop_session - pi_user_id: ' || pi_user_id
                || '; pi_guid: ' || pi_guid
                || chr(10) || sqlerrm);
    END p_create_desktop_session;
-------------------------------------------------------------------------
    FUNCTION f_get_roles (
        pi_user_id  IN app_users.user_id%type
    ) RETURN VARCHAR2 IS
        v_roles VARCHAR2(4000);
    BEGIN
        select
            trim(',' from 
                decode(is_sys_admin, 1, 'Administrator,', null)
                || decode(is_manager, 1, 'Manager,', null)
                || decode(is_operator, 1, 'Operator', null))
        into
            v_roles
        from
            app_users
        where   user_id = pi_user_id
            and deleted is null;
        return v_roles;
    EXCEPTION
        when no_data_found then
            return null;
        when others then
            raise_application_error(
                -20001,
                'f_get_roles - pi_user_id: ' || pi_user_id
                || chr(10) || sqlerrm);
    END f_get_roles;
-------------------------------------------------------------------------
    PROCEDURE p_login (
        pi_user_id  IN app_users.user_id%type,
        pi_guid     IN user_sessions.web_guid%type,
        pi_platform IN VARCHAR2, --WEB,MOBILE,DESKTOP
        po_roles    OUT VARCHAR2
    ) IS
    BEGIN
        if pi_platform = 'WEB' then
            p_create_web_session(pi_user_id, pi_guid);
        elsif pi_platform = 'MOBILE' then
            p_create_mobile_session(pi_user_id, pi_guid);
        elsif pi_platform = 'DESKTOP' then
            p_create_desktop_session(pi_user_id, pi_guid);
        else
            raise no_data_found;
        end if;
        po_roles := f_get_roles(pi_user_id);
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_login - pi_user_id: ' || pi_user_id
                || '; pi_guid: ' || pi_guid
                || chr(10) || sqlerrm);
    END p_login;
-------------------------------------------------------------------------
END pkg_auth;
/