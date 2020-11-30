-------------------------------------------------------------------------
-- PKG_CASE_ACTORS: Perform CRUD operations on CASE_ACTORS
-------------------------------------------------------------------------

CREATE OR REPLACE PACKAGE pkg_case_actors IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_actors (
        pi_case_id  IN case_actors.case_id%type,
        po_cursor   OUT sys_refcursor
    );

    PROCEDURE p_update_case_actors (
        pi_case_id          IN case_actors.case_id%type,
        pi_actor_type_id    IN case_actors.actor_type_id%type,
        pi_actor_name       IN case_actors.actor_name%type,
        pi_actor_address    IN case_actors.actor_address%type,
        pi_actor_email      IN case_actors.actor_email%type,
        pi_actor_phone      IN case_actors.actor_phone%type,
        pi_detail_file      IN case_actors.detail_file%type,
        pi_update_by        IN case_actors.last_update_by%type
    );

    PROCEDURE p_delete_case_actors (
        pi_case_id  IN case_actors.case_id%type
    );

END pkg_case_actors;
/
-------------------------------------------------------------------------

-------------------------------------------------------------------------
CREATE OR REPLACE PACKAGE BODY pkg_case_actors IS
-------------------------------------------------------------------------
    PROCEDURE p_get_all_case_actors (
        pi_case_id  IN case_actors.case_id%type,
        po_cursor   OUT sys_refcursor
    ) IS
    BEGIN
        open po_cursor for
            select
                case_id,
                actor_type_id,
                actor_name,
                actor_address,
                actor_email,
                actor_phone,
                detail_file
            from
                case_actors
            where   case_id = pi_case_id
                and deleted is null;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_get_all_case_actors - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_get_all_case_actors;
-------------------------------------------------------------------------
    PROCEDURE p_update_case_actors (
        pi_case_id          IN case_actors.case_id%type,
        pi_actor_type_id    IN case_actors.actor_type_id%type,
        pi_actor_name       IN case_actors.actor_name%type,
        pi_actor_address    IN case_actors.actor_address%type,
        pi_actor_email      IN case_actors.actor_email%type,
        pi_actor_phone      IN case_actors.actor_phone%type,
        pi_detail_file      IN case_actors.detail_file%type,
        pi_update_by        IN case_actors.last_update_by%type
    ) IS
    BEGIN
        update case_actors
        set actor_name = pi_actor_name,
            actor_address = pi_actor_address,
            actor_email = pi_actor_email,
            actor_phone = pi_actor_phone,
            detail_file = pi_detail_file,
            last_update_by = pi_update_by
        where case_id = pi_case_id
          and actor_type_id = pi_actor_type_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_update_case_actors - pi_case_id: ' || pi_case_id
                || '; pi_actor_type_id: ' || pi_actor_type_id
                || '; pi_actor_name: ' || pi_actor_name
                || '; pi_actor_address: ' || pi_actor_address
                || '; pi_actor_email: ' || pi_actor_email
                || '; pi_actor_phone: ' || pi_actor_phone
                || '; pi_detail_file: ' || pi_detail_file
                || '; pi_update_by: ' || pi_update_by
                || chr(10) || sqlerrm);
    END p_update_case_actors;
-------------------------------------------------------------------------
    PROCEDURE p_delete_case_actors (
        pi_case_id  IN case_actors.case_id%type
    ) IS
    BEGIN
        update case_actors
        set deleted = sysdate
        where case_id = pi_case_id;
    EXCEPTION
        when others then
            raise_application_error(
                -20001,
                'p_delete_case_actors - pi_case_id: ' || pi_case_id
                || chr(10) || sqlerrm);
    END p_delete_case_actors;
-------------------------------------------------------------------------
END pkg_case_actors;
/
-------------------------------------------------------------------------